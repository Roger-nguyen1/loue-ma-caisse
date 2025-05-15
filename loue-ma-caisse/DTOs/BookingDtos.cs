namespace loue_ma_caisse.DTOs;

public record CreateBookingDto(
    string VehicleId,
    DateTime StartDate,
    DateTime EndDate
);

public record BookingResponseDto(
    string Id,
    string VehicleId,
    string RenterId,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalPrice,
    string Status
);