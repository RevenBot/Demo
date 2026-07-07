using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace WebAPI.Models
{
    [Collection("restaurants")]
    public class Restaurant
    {

        // Primary key - string for URL/API convenience, stored as native ObjectId in DB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

        [Required(ErrorMessage = "You must provide a name")]
        [Display(Name = "Name")]
        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "You must add a cuisine type")]
        [Display(Name = "Cuisine")]
        [BsonElement("cuisine")]
        [JsonPropertyName("cuisine")]
        public string? Cuisine { get; set; }

        [Required(ErrorMessage = "You must add the borough of the restaurant")]
        [BsonElement("borough")]
        [JsonPropertyName("borough")]
        public string? Borough { get; set; }
    }
}
