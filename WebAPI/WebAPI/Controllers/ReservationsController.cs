using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : Controller
    {
        private readonly IReservationService _ReservationService;

        public ReservationsController(IReservationService ReservationService)
        {
            _ReservationService = ReservationService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await Task.FromResult( _ReservationService.GetAllReservations().ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(ObjectId id)
        {
            var product = _ReservationService.GetReservationById(id);
            if(product is null)
            {
                return NotFound();
            }

            return await Task.FromResult(product);
        }

        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation restaurant)
        {
            _ReservationService.AddReservation(restaurant);

            await Task.CompletedTask;

            return CreatedAtAction("GetReservation", new { id = restaurant.Id }, restaurant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(ObjectId id, Reservation restaurant)
        {
            if (id != restaurant.Id)
            {
                return BadRequest();
            }

            _ReservationService.EditReservation(restaurant);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(ObjectId id)
        {
            var product = _ReservationService.GetReservationById(id);
            if(product is null)
            {
                return NotFound();
            }
            _ReservationService.DeleteReservation(product);
            return Ok();
        }

        private bool ProductExists(int id)
        {
            return false;
        }

    }
}
