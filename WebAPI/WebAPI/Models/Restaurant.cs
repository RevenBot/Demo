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

        [BsonId]
        public ObjectId Id { get; set; }
        [Required(ErrorMessage = "You must provide a name")]
        [Display(Name = "Name")]
        public string? name { get; set; }


        [Required(ErrorMessage = "You must add a cuisine type")]
        [Display(Name = "Cuisine")]
        public string? cuisine { get; set; }


        [Required(ErrorMessage = "You must add the borough of the restaurant")]
        public string? borough { get; set; }
        [JsonPropertyName("_id")]
        public string _Id
        {
            get
            {
                return Id.ToString();
            }
        }
    }
}
