using MongoDB.Driver;
using MongoDB.Bson;
using loue_ma_caisse.Models;
using loue_ma_caisse.DTOs;

namespace loue_ma_caisse.Services;

public interface IVehicleService
{
    Task<IEnumerable<Vehicle>> GetVehiclesAsync(VehicleFilterDto filter);
    Task<Vehicle> GetVehicleByIdAsync(string id);
    Task<Vehicle> CreateVehicleAsync(string ownerId, CreateVehicleDto vehicleDto);
    Task<Vehicle> UpdateVehicleAsync(string id, string ownerId, UpdateVehicleDto vehicleDto);
    Task DeleteVehicleAsync(string id, string ownerId);
    Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(string userId);
}

public class VehicleService : IVehicleService
{
    private readonly IMongoCollection<Vehicle> _vehicles;

    public VehicleService(MongoDbService mongoDbService)
    {
        _vehicles = mongoDbService.GetCollection<Vehicle>("Vehicles");
    }

    public async Task<IEnumerable<Vehicle>> GetVehiclesAsync(VehicleFilterDto filter)
    {
        var builder = Builders<Vehicle>.Filter;
        var filterDefinition = builder.Empty;

        if (!string.IsNullOrEmpty(filter.City))
            filterDefinition &= builder.Eq(v => v.City, filter.City);

        if (filter.MaxPrice.HasValue)
            filterDefinition &= builder.Lte(v => v.PricePerDay, filter.MaxPrice.Value);

        if (!string.IsNullOrEmpty(filter.Transmission))
            filterDefinition &= builder.Eq(v => v.Transmission, filter.Transmission);

        if (!string.IsNullOrEmpty(filter.FuelType))
            filterDefinition &= builder.Eq(v => v.FuelType, filter.FuelType);

        if (filter.IsElectric.HasValue)
            filterDefinition &= builder.Eq(v => v.IsElectric, filter.IsElectric.Value);

        filterDefinition &= builder.Eq(v => v.Available, true);

        return await _vehicles.Find(filterDefinition).ToListAsync();
    }

    public async Task<Vehicle> GetVehicleByIdAsync(string id)
    {
        return await _vehicles.Find(v => v.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Vehicle> CreateVehicleAsync(string ownerId, CreateVehicleDto vehicleDto)
    {
        var vehicle = new Vehicle
        {
            Id = ObjectId.GenerateNewId().ToString(),
            OwnerId = ownerId,
            Brand = vehicleDto.Brand,
            Model = vehicleDto.Model,
            Year = vehicleDto.Year,
            Transmission = vehicleDto.Transmission,
            FuelType = vehicleDto.FuelType,
            IsElectric = vehicleDto.IsElectric,
            City = vehicleDto.City,            PricePerDay = vehicleDto.PricePerDay,
            ImageUrl = vehicleDto.ImageUrl,
            Available = true,
            CreatedAt = DateTime.UtcNow
        };

        await _vehicles.InsertOneAsync(vehicle);
        return vehicle;
    }

    public async Task<Vehicle> UpdateVehicleAsync(string id, string ownerId, UpdateVehicleDto vehicleDto)
    {
        var filter = Builders<Vehicle>.Filter.And(
            Builders<Vehicle>.Filter.Eq(v => v.Id, id),
            Builders<Vehicle>.Filter.Eq(v => v.OwnerId, ownerId)
        );

        var update = Builders<Vehicle>.Update
            .Set(v => v.Brand, vehicleDto.Brand)
            .Set(v => v.Model, vehicleDto.Model)
            .Set(v => v.Year, vehicleDto.Year)
            .Set(v => v.Transmission, vehicleDto.Transmission)
            .Set(v => v.FuelType, vehicleDto.FuelType)            .Set(v => v.IsElectric, vehicleDto.IsElectric)
            .Set(v => v.City, vehicleDto.City)
            .Set(v => v.PricePerDay, vehicleDto.PricePerDay)
            .Set(v => v.Available, vehicleDto.Available)
            .Set(v => v.ImageUrl, vehicleDto.ImageUrl);

        return await _vehicles.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Vehicle> { ReturnDocument = ReturnDocument.After });
    }

    public async Task DeleteVehicleAsync(string id, string ownerId)
    {
        var filter = Builders<Vehicle>.Filter.And(
            Builders<Vehicle>.Filter.Eq(v => v.Id, id),
            Builders<Vehicle>.Filter.Eq(v => v.OwnerId, ownerId)
        );

        await _vehicles.DeleteOneAsync(filter);
    }    public async Task<IEnumerable<Vehicle>> GetUserVehiclesAsync(string userId)
    {
        Console.WriteLine($"Recherche des véhicules pour l'utilisateur: {userId}");
        
        var filter = Builders<Vehicle>.Filter.Eq(v => v.OwnerId, userId);
        var sort = Builders<Vehicle>.Sort.Descending(v => v.CreatedAt);
        
        var vehicles = await _vehicles.Find(filter)
            .Sort(sort)
            .ToListAsync();
            
        Console.WriteLine($"Nombre de véhicules trouvés: {vehicles.Count()}");
        return vehicles;
    }
}