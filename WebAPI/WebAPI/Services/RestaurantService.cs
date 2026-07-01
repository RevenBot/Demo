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

        public void AddRestaurant(Restaurant restaurant)
        {
            // Id already set via default value in model, no need to generate here
            _restaurantDbContext.Restaurants.Add(restaurant);
            _restaurantDbContext.ChangeTracker.DetectChanges();
            Console.WriteLine(_restaurantDbContext.ChangeTracker.DebugView.LongView);
            _restaurantDbContext.SaveChanges();
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

        public void EditRestaurant(Restaurant updated)
        {
            var existing = _restaurantDbContext.Restaurants.FirstOrDefault(c => c.Id == updated.Id);
            
            if (existing != null)
            {
                // Only update fields that were actually provided in the body
                if (!string.IsNullOrEmpty(updated.Name))
                    existing.Name = updated.Name;
                if (!string.IsNullOrEmpty(updated.Cuisine))
                    existing.Cuisine = updated.Cuisine;
                if (!string.IsNullOrEmpty(updated.Borough))
                    existing.Borough = updated.Borough;

                _restaurantDbContext.Restaurants.Update(existing);
                _restaurantDbContext.ChangeTracker.DetectChanges();
                Console.WriteLine(_restaurantDbContext.ChangeTracker.DebugView.LongView);
                _restaurantDbContext.SaveChanges();
            }
            else
            {
                throw new ArgumentException("The restaurant to update cannot be found.");
            }
        }

        public IEnumerable<Restaurant> GetAllRestaurants()
        {
            return _restaurantDbContext.Restaurants.OrderByDescending(c => c.Id).Take(20).AsNoTracking().ToList();
        }

        // Now accepts string ID directly - no ObjectId conversion needed
        public Restaurant? GetRestaurantById(string id)
        {
            return _restaurantDbContext.Restaurants.FirstOrDefault(c => c.Id == id);
        }
    }
}
