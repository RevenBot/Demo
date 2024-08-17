using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService _RestaurantService;

        public RestaurantsController(IRestaurantService RestaurantService)
        {
            _RestaurantService = RestaurantService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            return await Task.FromResult( _RestaurantService.GetAllRestaurants().ToList());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(ObjectId id)
        {
            var product = _RestaurantService.GetRestaurantById(id);
            if(product is null)
            {
                return NotFound();
            }

            return await Task.FromResult(product);
        }

        [HttpPost]
        public async Task<ActionResult<Restaurant>> PostRestaurant(Restaurant restaurant)
        {
            _RestaurantService.AddRestaurant(restaurant);

            await Task.CompletedTask;

            return CreatedAtAction("GetRestaurant", new { id = restaurant.Id }, restaurant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(ObjectId id, Restaurant restaurant)
        {
            if (id != restaurant.Id)
            {
                return BadRequest();
            }

            _RestaurantService.EditRestaurant(restaurant);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(ObjectId id)
        {
            var product = _RestaurantService.GetRestaurantById(id);
            product.Id.ToString();
            if(product is null)
            {
                return NotFound();
            }
            _RestaurantService.DeleteRestaurant(product);
            return Ok();
        }

        private bool ProductExists(int id)
        {
            return false;
        }
    }
}
