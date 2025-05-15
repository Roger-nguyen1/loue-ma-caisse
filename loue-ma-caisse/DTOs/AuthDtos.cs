namespace loue_ma_caisse.DTOs;

public record RegisterUserDto(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string PhoneNumber
);

public record LoginUserDto(
    string Email,
    string Password
);

public record AuthResponseDto(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Token
);