using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dto;

public class ProductInputDto
{
    [Required(ErrorMessage = "You must provide a name")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "You must add a price")]
    public decimal Price { get; set; }
}
