using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IRestaurantService
    {
        IEnumerable<Restaurant> GetAllRestaurants();
        Restaurant? GetRestaurantById(string id);

        void AddRestaurant(Restaurant newRestaurant);

        void EditRestaurant(Restaurant updatedRestaurant);

        void DeleteRestaurant(Restaurant restaurantToDelete);
    }
}
