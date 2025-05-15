using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace loue_ma_caisse.Models;

public class Vehicle
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; } = null!;

    [BsonElement("ownerId")]
    public required string OwnerId { get; set; }

    [BsonElement("brand")]
    public required string Brand { get; set; }

    [BsonElement("model")]
    public required string Model { get; set; }

    [BsonElement("year")]
    public int Year { get; set; }

    [BsonElement("transmission")]
    public required string Transmission { get; set; } // "Manual" ou "Automatic"

    [BsonElement("fuelType")]
    public required string FuelType { get; set; } // "Essence", "Diesel", "Électrique", "Hybride"

    [BsonElement("isElectric")]
    public bool IsElectric { get; set; }

    [BsonElement("city")]
    public required string City { get; set; }

    [BsonElement("pricePerDay")]
    public decimal PricePerDay { get; set; }

    [BsonElement("available")]
    public bool Available { get; set; } = true;    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
}