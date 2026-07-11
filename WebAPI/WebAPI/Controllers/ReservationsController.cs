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
        
        // GET /api/reservations - returns list of reservations with id fields and pagination
        [HttpGet]
        public async Task<ActionResult<PagedResult<ReservationOutputDto>>> GetReservations([FromQuery] PaginationParamsDto paramsDto)
        {
            var skip = (paramsDto.PageNumber - 1) * paramsDto.PageSize;

            var pagedResult = await _reservationService.GetAllReservationsAsync(skip, paramsDto.PageSize);

            return Ok(new PagedResult<ReservationOutputDto>
            {
                Items = pagedResult.Items.Select(r => new ReservationOutputDto
                {
                    Id = r.Id,
                    RestaurantId = r.RestaurantId,
                    RestaurantName = r.RestaurantName,
                    Date = r.Date
                }),
                TotalCount = pagedResult.TotalCount,
                PageSize = pagedResult.PageSize,
                CurrentPage = pagedResult.CurrentPage
            });
        }

        // GET /api/reservations/{id} - returns single reservation with id field
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationOutputDto>> GetReservation(string id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
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

            var created = await _reservationService.AddReservationAsync(reservation);

            return CreatedAtAction("GetReservation", 
                new { id = created.Id }, 
                new ReservationOutputDto
                {
                    Id = created.Id,
                    RestaurantId = created.RestaurantId,
                    RestaurantName = created.RestaurantName,
                    Date = created.Date
                });
        }

        // PUT /api/reservations/{id} - updates via string path param (no id in body)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(string id, [FromBody] ReservationInputDto input)
        {
            var existing = await _reservationService.GetReservationByIdAsync(id);
            if (existing is null) return NotFound();

            existing.Date = input.Date;

            var updated = await _reservationService.EditReservationAsync(existing);

            ReservationOutputDto reservationOutputDto = new ReservationOutputDto
            {
                Id = updated.Id,
                RestaurantId = updated.RestaurantId,
                RestaurantName = updated.RestaurantName,
                Date = updated.Date
            };

            return Ok(reservationOutputDto);
        }

        // DELETE /api/reservations/{id} - deletes via string path param
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(string id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation is null) return NotFound();

            await _reservationService.DeleteReservationAsync(reservation);
            return Ok();
        }
    }
}
