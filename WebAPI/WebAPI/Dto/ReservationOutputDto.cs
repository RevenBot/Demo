namespace WebAPI.Dto;

public class ReservationOutputDto
{
    public string Id { get; set; } = "";

    public string? RestaurantId { get; set; }

    public string? RestaurantName { get; set; }

    public DateTime Date { get; set; }
}
