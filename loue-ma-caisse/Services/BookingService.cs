using MongoDB.Driver;
using MongoDB.Bson;
using loue_ma_caisse.Models;
using loue_ma_caisse.DTOs;

namespace loue_ma_caisse.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(string renterId, CreateBookingDto bookingDto);
    Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(string userId);
    Task<BookingResponseDto?> GetBookingByIdAsync(string id);
    Task CancelBookingAsync(string bookingId, string userId);
}

public class BookingService : IBookingService
{
    private readonly IMongoCollection<Booking> _bookings;
    private readonly IVehicleService _vehicleService;

    public BookingService(MongoDbService mongoDbService, IVehicleService vehicleService)
    {
        _bookings = mongoDbService.GetCollection<Booking>("Bookings");
        _vehicleService = vehicleService;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(string renterId, CreateBookingDto bookingDto)
    {
        var vehicle = await _vehicleService.GetVehicleByIdAsync(bookingDto.VehicleId);
        if (vehicle == null)
            throw new Exception("Véhicule non trouvé");
        
        if (!vehicle.Available)
            throw new Exception("Ce véhicule n'est pas disponible");

        // Vérifier s'il n'y a pas déjà une réservation pour ces dates
        var existingBooking = await _bookings.Find(b => 
            b.VehicleId == bookingDto.VehicleId &&
            b.Status != "Cancelled" &&
            ((b.StartDate <= bookingDto.StartDate && b.EndDate >= bookingDto.StartDate) ||
             (b.StartDate <= bookingDto.EndDate && b.EndDate >= bookingDto.EndDate))).FirstOrDefaultAsync();

        if (existingBooking != null)
            throw new Exception("Le véhicule est déjà réservé pour ces dates");

        var numberOfDays = (bookingDto.EndDate - bookingDto.StartDate).Days;
        var totalPrice = numberOfDays * vehicle.PricePerDay;

        var booking = new Booking
        {
            Id = ObjectId.GenerateNewId().ToString(),
            VehicleId = bookingDto.VehicleId,
            RenterId = renterId,
            StartDate = bookingDto.StartDate,
            EndDate = bookingDto.EndDate,
            TotalPrice = totalPrice,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        await _bookings.InsertOneAsync(booking);

        return new BookingResponseDto(
            booking.Id,
            booking.VehicleId,
            booking.RenterId,
            booking.StartDate,
            booking.EndDate,
            booking.TotalPrice,
            booking.Status
        );
    }

    public async Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(string userId)
    {
        var bookings = await _bookings.Find(b => b.RenterId == userId)
            .SortByDescending(b => b.CreatedAt)
            .ToListAsync();

        return bookings.Select(b => new BookingResponseDto(
            b.Id,
            b.VehicleId,
            b.RenterId,
            b.StartDate,
            b.EndDate,
            b.TotalPrice,
            b.Status
        ));
    }

    public async Task<BookingResponseDto?> GetBookingByIdAsync(string id)
    {
        var booking = await _bookings.Find(b => b.Id == id).FirstOrDefaultAsync();
        if (booking == null)
            return null;

        return new BookingResponseDto(
            booking.Id,
            booking.VehicleId,
            booking.RenterId,
            booking.StartDate,
            booking.EndDate,
            booking.TotalPrice,
            booking.Status
        );
    }

    public async Task CancelBookingAsync(string bookingId, string userId)
    {
        var filter = Builders<Booking>.Filter.And(
            Builders<Booking>.Filter.Eq(b => b.Id, bookingId),
            Builders<Booking>.Filter.Eq(b => b.RenterId, userId),
            Builders<Booking>.Filter.Ne(b => b.Status, "Completed")
        );

        var update = Builders<Booking>.Update
            .Set(b => b.Status, "Cancelled");

        var result = await _bookings.UpdateOneAsync(filter, update);
        
        if (result.ModifiedCount == 0)
            throw new Exception("Impossible d'annuler cette réservation");
    }
}