using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace loue_ma_caisse.Models;

public class Booking
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string Id { get; set; } = null!;

    [BsonElement("vehicleId")]
    public required string VehicleId { get; set; }

    [BsonElement("renterId")]
    public required string RenterId { get; set; }

    [BsonElement("startDate")]
    public DateTime StartDate { get; set; }

    [BsonElement("endDate")]
    public DateTime EndDate { get; set; }

    [BsonElement("totalPrice")]
    public decimal TotalPrice { get; set; }

    [BsonElement("status")]
    public required string Status { get; set; } // "Pending", "Confirmed", "Cancelled", "Completed"

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}