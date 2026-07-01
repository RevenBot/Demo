using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Dto;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }
        
        // GET /api/reservations - returns list of reservations with id fields
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationOutputDto>>> GetReservations()
        {
            var reservations = await Task.FromResult(_reservationService.GetAllReservations().ToList());
            return Ok(reservations.Select(r => new ReservationOutputDto
            {
                Id = r.Id,
                RestaurantId = r.RestaurantId,
                RestaurantName = r.RestaurantName,
                Date = r.Date
            }));
        }

        // GET /api/reservations/{id} - returns single reservation with id field
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationOutputDto>> GetReservation(string id)
        {
            var reservation = _reservationService.GetReservationById(id);
            if (reservation is null) return NotFound();

            return Ok(new ReservationOutputDto
            {
                Id = reservation.Id,
                RestaurantId = reservation.RestaurantId,
                RestaurantName = reservation.RestaurantName,
                Date = reservation.Date
            });
        }

        // POST /api/reservations - no id in input (server auto-generates)
        [HttpPost]
        public async Task<ActionResult<ReservationOutputDto>> PostReservation(ReservationInputDto input)
        {
            var reservation = new Reservation
            {
                RestaurantId = input.RestaurantId,
                Date = input.Date
            };

            _reservationService.AddReservation(reservation);
            await Task.CompletedTask;

            return CreatedAtAction("GetReservation", 
                new { id = reservation.Id }, 
                new ReservationOutputDto
                {
                    Id = reservation.Id,
                    RestaurantId = reservation.RestaurantId,
                    RestaurantName = reservation.RestaurantName,
                    Date = reservation.Date
                });
        }

        // PUT /api/reservations/{id} - updates via string path param (no id in body)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(string id, [FromBody] ReservationInputDto input)
        {
            var existing = _reservationService.GetReservationById(id);
            if (existing is null) return NotFound();

            // RestaurantName is auto-populated server-side; only update Date from input
            if (input.Date != default)
                existing.Date = input.Date;

            _reservationService.EditReservation(existing);
            return Ok();
        }

        // DELETE /api/reservations/{id} - deletes via string path param
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(string id)
        {
            var reservation = _reservationService.GetReservationById(id);
            if (reservation is null) return NotFound();

            _reservationService.DeleteReservation(reservation);
            return Ok();
        }
    }
}
