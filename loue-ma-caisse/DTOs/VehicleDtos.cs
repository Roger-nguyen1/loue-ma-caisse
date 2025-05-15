namespace loue_ma_caisse.DTOs;

public record CreateVehicleDto(
    string Brand,
    string Model,
    int Year,
    string Transmission,
    string FuelType,
    bool IsElectric,
    string City,
    decimal PricePerDay,
    string ImageUrl
);

public record UpdateVehicleDto(
    string Brand,
    string Model,
    int Year,
    string Transmission,
    string FuelType,
    bool IsElectric,
    string City,
    decimal PricePerDay,
    bool Available,
    string ImageUrl
);

public record VehicleFilterDto(
    string? City = null,
    decimal? MaxPrice = null,
    string? Transmission = null,
    string? FuelType = null,
    bool? IsElectric = null
);