using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MongoDB.Bson;
using BCrypt.Net;
using loue_ma_caisse.Models;
using loue_ma_caisse.DTOs;

namespace loue_ma_caisse.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginUserDto loginDto);
}

public class AuthService : IAuthService
{
    private readonly IMongoCollection<User> _users;
    private readonly IConfiguration _configuration;

    public AuthService(MongoDbService mongoDbService, IConfiguration configuration)
    {
        _users = mongoDbService.GetCollection<User>("Users");
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
    {
        // Vérifier si l'email existe déjà
        var existingUser = await _users.Find(u => u.Email == registerDto.Email).FirstOrDefaultAsync();
        if (existingUser != null)
            throw new Exception("Un utilisateur avec cet email existe déjà");

        // Créer le nouvel utilisateur
        var user = new User
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Email = registerDto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        await _users.InsertOneAsync(user);

        // Générer le token JWT
        var token = GenerateJwtToken(user);

        return new AuthResponseDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token
        );
    }

    public async Task<AuthResponseDto> LoginAsync(LoginUserDto loginDto)
    {
        var user = await _users.Find(u => u.Email == loginDto.Email).FirstOrDefaultAsync();
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password))
            throw new Exception("Email ou mot de passe incorrect");

        var token = GenerateJwtToken(user);

        return new AuthResponseDto(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            token
        );
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["JwtSettings:SecretKey"] ?? 
            throw new InvalidOperationException("La clé JWT n'est pas configurée");
            
        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName),
                new Claim(ClaimTypes.Surname, user.LastName)
            }),
            Expires = DateTime.UtcNow.AddDays(7), // Token valide 7 jours
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}