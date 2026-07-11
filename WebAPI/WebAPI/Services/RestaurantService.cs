using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantReservationDbContext _restaurantDbContext;
        
        public RestaurantService(RestaurantReservationDbContext restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }

        public async Task<Restaurant> AddRestaurantAsync(Restaurant restaurant)
        {
            // Id already set via default value in model, no need to generate here
            await _restaurantDbContext.Restaurants.AddAsync(restaurant);
            await _restaurantDbContext.SaveChangesAsync();
            return restaurant;
        }

        public async Task DeleteRestaurantAsync(Restaurant restaurant)
        {
            var restaurantToDelete = await _restaurantDbContext.Restaurants.FirstOrDefaultAsync(c => c.Id == restaurant.Id);
            
            if (restaurantToDelete != null)
            {
                _restaurantDbContext.Restaurants.Remove(restaurantToDelete);
                _restaurantDbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_restaurantDbContext.ChangeTracker.DebugView.LongView);
                await _restaurantDbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException("The restaurant to delete cannot be found.");
            }
        }

        public async Task<Restaurant> EditRestaurantAsync(Restaurant updated)
        {
            _restaurantDbContext.Restaurants.Update(updated);
            await _restaurantDbContext.SaveChangesAsync();
            return updated;
        }

        public async Task<PagedResult<Restaurant>> GetAllRestaurantsAsync(int skip, int take)
        {
            int totalCount = await _restaurantDbContext.Restaurants.CountAsync();
            var items = await _restaurantDbContext.Restaurants.OrderByDescending(r => r.Id).Skip(skip).Take(take).ToListAsync();
            return new PagedResult<Restaurant> { Items = items, TotalCount = totalCount, PageSize = take, CurrentPage = (skip / take) + 1 };
        }

        // Now accepts string ID directly - no ObjectId conversion needed
        public async Task<Restaurant?> GetRestaurantByIdAsync(string id)
        {
            return await _restaurantDbContext.Restaurants.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
