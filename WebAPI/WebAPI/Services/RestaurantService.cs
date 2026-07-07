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

        public Restaurant AddRestaurant(Restaurant restaurant)
        {
            // Id already set via default value in model, no need to generate here
            _restaurantDbContext.Restaurants.Add(restaurant);
            _restaurantDbContext.SaveChanges();
            return restaurant;
        }

        public void DeleteRestaurant(Restaurant restaurant)
        {
            var restaurantToDelete = _restaurantDbContext.Restaurants.Where(c => c.Id == restaurant.Id).FirstOrDefault();
            
            if (restaurantToDelete != null)
            {
                _restaurantDbContext.Restaurants.Remove(restaurantToDelete);
                _restaurantDbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_restaurantDbContext.ChangeTracker.DebugView.LongView);
                _restaurantDbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The restaurant to delete cannot be found.");
            }
        }

        public Restaurant EditRestaurant(Restaurant updated)
        {
            _restaurantDbContext.Restaurants.Update(updated);
            _restaurantDbContext.SaveChanges();
            return updated;
        }

        public PagedResult<Restaurant> GetAllRestaurants(int skip, int take)
        {
            int totalCount = _restaurantDbContext.Restaurants.Count();
            var items = _restaurantDbContext.Restaurants.OrderByDescending(r => r.Id).Skip(skip).Take(take).ToList();
            return new PagedResult<Restaurant> { Items = items, TotalCount = totalCount, PageSize = take, CurrentPage = (skip / take) + 1 };
        }

        // Now accepts string ID directly - no ObjectId conversion needed
        public Restaurant? GetRestaurantById(string id)
        {
            return _restaurantDbContext.Restaurants.FirstOrDefault(c => c.Id == id);
        }
    }
}
