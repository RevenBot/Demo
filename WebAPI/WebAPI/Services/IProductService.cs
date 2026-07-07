using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IProductService
    {
        Task<PagedResult<Product>> GetAllAsync(int skip, int take);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
