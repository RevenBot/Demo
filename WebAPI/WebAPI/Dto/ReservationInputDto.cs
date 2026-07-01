using System.ComponentModel.DataAnnotations;

namespace WebAPI.Dto;

public class ReservationInputDto
{
    public string? RestaurantId { get; set; }

    [Required(ErrorMessage = "The date and time is required to make this reservation")]
    public DateTime Date { get; set; }
}
