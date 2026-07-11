using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace WebAPI.Models
{
    [Collection("reservations")]
    public class Reservation
    {
        // Primary key - string for URL/API convenience, stored as native ObjectId in DB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        // Foreign key to restaurant (native ObjectId stored in MongoDB)
        [BsonElement("restaurantId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("restaurantId")]
        public string? RestaurantId { get; set; }

        // Cached name for display purposes - no need for ObjectId representation
        [BsonElement("restaurantName")]
        [JsonPropertyName("restaurantName")]
        public string? RestaurantName { get; set; }

        // Date/time field with explicit BSON type mapping
        [Required(ErrorMessage = "The date and time is required to make this reservation")]
        [Display(Name = "Date")]
        [BsonElement("date")]
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

    }
}
