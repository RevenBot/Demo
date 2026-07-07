using WebAPI.Models;

namespace WebAPI.Services
{
    public interface IProductService
    {
        PagedResult<Product> GetAll(int skip, int take);
        Product? GetById(int id);
        Product Add(Product product);
        Product Update(Product product);
        void Delete(int id);
    }
}
