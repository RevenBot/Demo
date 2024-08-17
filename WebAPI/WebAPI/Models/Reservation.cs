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
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? RestaurantId { get; set; }


        public string? RestaurantName { get; set; }

        [Required(ErrorMessage = "The date and time is required to make this reservation")]
        [Display(Name = "Date")]
        public DateTime date { get; set; }

        [JsonPropertyName("_id")]
        public string _Id
        {
            get
            {
                return Id.ToString();
            }
        }
        [JsonPropertyName("_restaurantId")]
        public string _RestaurantId
        {
            get
            {
                return RestaurantId.ToString() ?? string.Empty;
            }
        }


    }
}
