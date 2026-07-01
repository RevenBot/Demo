using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Dto;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : Controller
    {
        private readonly IRestaurantService _restaurantService;

        public RestaurantsController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }
        
        // GET /api/restaurants - returns list of restaurants with id fields
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RestaurantOutputDto>>> GetRestaurants()
        {
            var restaurants = await Task.FromResult(_restaurantService.GetAllRestaurants().ToList());
            return Ok(restaurants.Select(r => new RestaurantOutputDto
            {
                Id = r.Id,
                Name = r.Name,
                Cuisine = r.Cuisine,
                Borough = r.Borough
            }));
        }

        // GET /api/restaurants/{id} - returns single restaurant with id field
        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantOutputDto>> GetRestaurant(string id)
        {
            var restaurant = _restaurantService.GetRestaurantById(id);
            if (restaurant is null) return NotFound();

            return Ok(new RestaurantOutputDto
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Cuisine = restaurant.Cuisine,
                Borough = restaurant.Borough
            });
        }

        // POST /api/restaurants - no id in input (server auto-generates)
        [HttpPost]
        public async Task<ActionResult<RestaurantOutputDto>> PostRestaurant(RestaurantInputDto input)
        {
            var restaurant = new Restaurant
            {
                Name = input.Name,
                Cuisine = input.Cuisine,
                Borough = input.Borough
            };

            _restaurantService.AddRestaurant(restaurant);
            await Task.CompletedTask;

            return CreatedAtAction("GetRestaurant", 
                new { id = restaurant.Id }, 
                new RestaurantOutputDto
                {
                    Id = restaurant.Id,
                    Name = restaurant.Name,
                    Cuisine = restaurant.Cuisine,
                    Borough = restaurant.Borough
                });
        }

        // PUT /api/restaurants/{id} - updates via string path param (no id in body)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(string id, [FromBody] RestaurantInputDto input)
        {
            var existing = _restaurantService.GetRestaurantById(id);
            if (existing is null) return NotFound();

            if (!string.IsNullOrEmpty(input.Name))
                existing.Name = input.Name;
            if (!string.IsNullOrEmpty(input.Cuisine))
                existing.Cuisine = input.Cuisine;
            if (!string.IsNullOrEmpty(input.Borough))
                existing.Borough = input.Borough;

            _restaurantService.EditRestaurant(existing);
            return Ok();
        }

        // DELETE /api/restaurants/{id} - deletes via string path param
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(string id)
        {
            var restaurant = _restaurantService.GetRestaurantById(id);
            if (restaurant is null) return NotFound();

            _restaurantService.DeleteRestaurant(restaurant);
            return Ok();
        }
    }
}
