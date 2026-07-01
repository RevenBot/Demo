using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dto;

public class RestaurantInputDto
{
    [Required(ErrorMessage = "You must provide a name")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "You must add a cuisine type")]
    public string Cuisine { get; set; } = "";

    [Required(ErrorMessage = "You must add the borough of the restaurant")]
    public string Borough { get; set; } = "";
}
