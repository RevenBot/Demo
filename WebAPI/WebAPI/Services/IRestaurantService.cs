using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IRestaurantService
    {
        PagedResult<Restaurant> GetAllRestaurants(int skip, int take);
        Restaurant? GetRestaurantById(string id);

        Restaurant AddRestaurant(Restaurant newRestaurant);

        Restaurant EditRestaurant(Restaurant updatedRestaurant);

        void DeleteRestaurant(Restaurant restaurantToDelete);
    }
}
