using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IRestaurantService
    {
        Task<PagedResult<Restaurant>> GetAllRestaurantsAsync(int skip, int take);
        Task<Restaurant?> GetRestaurantByIdAsync(string id);

        Task<Restaurant> AddRestaurantAsync(Restaurant newRestaurant);

        Task<Restaurant> EditRestaurantAsync(Restaurant updatedRestaurant);

        Task DeleteRestaurantAsync(Restaurant restaurantToDelete);
    }
}
