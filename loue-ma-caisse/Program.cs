using dotenv.net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using loue_ma_caisse.Services;
using loue_ma_caisse.Validators;

Console.WriteLine("Chargement de l'app...chargement de dotenv");
DotEnv.Load();
Console.WriteLine("DotEnv. OK");

var builder = WebApplication.CreateBuilder(args);

// Configuration CORS avec origines multiples selon l'environnement
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Liste des origines autorisées
        var allowedOrigins = new List<string>
        {
            "http://localhost:3000",
            "https://localhost:3000",
            "http://localhost:3001",
            "https://localhost:3001",
            "https://loue-ma-caisse.vercel.app",
            "https://loue-ma-voiture-api.rogernguyen.fr",  
            "http://loue-ma-voiture-api.rogernguyen.fr"  
        };

        policy.WithOrigins(allowedOrigins.ToArray())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddSingleton<MongoDbService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Add Validators
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserDtoValidator>();

builder.Services.AddControllers();

// Configure JWT Authentication
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];
if (string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("La clé JWT n'est pas configurée dans appsettings.json");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey)
            )
        };
    });

// Configuration de Kestrel pour écouter sur le bon port
/*builder.WebHost.ConfigureKestrel(serverOptions =>
{
    // En production (Coolify), écoute sur le port 3005
    //if (!builder.Environment.IsDevelopment())
    {
        serverOptions.ListenAnyIP(3005); // Port utilisé par Traefik
    }
    // En développement, utilise les ports par défaut (ou spécifiés dans launchSettings.json)
});*/

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Loue Ma Caisse API", 
        Version = "v1",
        Description = "API de location de voitures entre particuliers"
    });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // En développement, utilise les certificats de développement
    app.UseDeveloperExceptionPage();
}
else 
{
    // En production, pas besoin de redirection HTTPS (géré par Traefik)
    // Donc on retire app.UseHttpsRedirection();
    app.UseExceptionHandler("/Error");
}

app.UseSwagger(c =>
{
    c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
    // Configuration de l'URL Swagger selon l'environnement
    if (app.Environment.IsDevelopment())
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Loue Ma Caisse API V1");
    }
    else
    {
        // En production, utilise l'URL complète
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Loue Ma Caisse API V1");
    }
    
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "Loue Ma Caisse API Documentation";
    c.EnableDeepLinking();
    c.DisplayRequestDuration();
});

// Ensure CORS is used before authentication
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
