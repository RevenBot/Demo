using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;

namespace WebAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public PagedResult<Product> GetAll(int skip, int take)
        {
            int totalCount = _context.Products.Count();
            List<Product> items = _context.Products.Skip(skip).Take(take).ToList();
            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = take,
                CurrentPage = (skip / take) + 1
            };
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public Product Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
            return product;
        }

        public Product Update(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
            return product;
        }

        public void Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }
    }
}
