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
        public async Task<ActionResult<PagedResult<RestaurantOutputDto>>> GetRestaurants([FromQuery] PaginationParamsDto paramsDto)
        {
            int skip = (paramsDto.PageNumber - 1) * paramsDto.PageSize;
            
            var result = await _restaurantService.GetAllRestaurantsAsync(skip, paramsDto.PageSize);

            return Ok(new PagedResult<RestaurantOutputDto>
            {
                Items = result.Items.Select(r => new RestaurantOutputDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Cuisine = r.Cuisine,
                    Borough = r.Borough
                }),
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                CurrentPage = result.CurrentPage
            });
        }

        // GET /api/restaurants/{id} - returns single restaurant with id field
        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantOutputDto>> GetRestaurant(string id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
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

            var created = await _restaurantService.AddRestaurantAsync(restaurant);

            return CreatedAtAction("GetRestaurant", 
                new { id = created.Id }, 
                new RestaurantOutputDto
                {
                    Id = created.Id,
                    Name = created.Name,
                    Cuisine = created.Cuisine,
                    Borough = created.Borough
                });
        }

        // PUT /api/restaurants/{id} - updates via string path param (no id in body)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRestaurant(string id, [FromBody] RestaurantInputDto input)
        {
            var existing = await _restaurantService.GetRestaurantByIdAsync(id);
            if (existing is null) return NotFound();

            if (!string.IsNullOrEmpty(input.Name))
                existing.Name = input.Name;
            if (!string.IsNullOrEmpty(input.Cuisine))
                existing.Cuisine = input.Cuisine;
            if (!string.IsNullOrEmpty(input.Borough))
                existing.Borough = input.Borough;

            var updated = await _restaurantService.EditRestaurantAsync(existing);

            return Ok(new RestaurantOutputDto
            {
                Id = updated.Id,
                Name = updated.Name,
                Cuisine = updated.Cuisine,
                Borough = updated.Borough
            });
        }

        // DELETE /api/restaurants/{id} - deletes via string path param
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRestaurant(string id)
        {
            var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
            if (restaurant is null) return NotFound();

            await _restaurantService.DeleteRestaurantAsync(restaurant);
            return Ok();
        }
    }
}
