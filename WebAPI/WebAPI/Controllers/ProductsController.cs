using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Services;
using WebAPI.Dto;

namespace WebAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET /api/products - returns list of products with id fields
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductOutputDto>>> GetProducts()
        {
            var products = await Task.FromResult(_productService.GetAll().ToList());
            return Ok(products.Select(p => new ProductOutputDto
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Price = p.Price
            }));
        }

        // GET /api/products/{id} - returns single product with id field
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductOutputDto>> GetProduct(string id)
        {
            var product = _productService.GetById(int.Parse(id));
            if (product is null) return NotFound();

            return Ok(new ProductOutputDto
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Price = product.Price
            });
        }

        // POST /api/products - no id in input (server auto-generates)
        [HttpPost]
        public async Task<ActionResult<ProductOutputDto>> PostProduct(ProductInputDto input)
        {
            var product = new Product
            {
                Name = input.Name,
                Price = input.Price
            };

            _productService.Add(product);
            await Task.CompletedTask;

            return CreatedAtAction("GetProduct", 
                new { id = product.Id }, 
                new ProductOutputDto
                {
                    Id = product.Id.ToString(),
                    Name = product.Name,
                    Price = product.Price
                });
        }

        // PUT /api/products/{id} - updates via string path param (no id in body)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(string id, [FromBody] ProductInputDto input)
        {
            var existing = _productService.GetById(int.Parse(id));
            if (existing is null) return NotFound();

            // Only update fields that were actually provided in the body
            if (!string.IsNullOrEmpty(input.Name))
                existing.Name = input.Name;
            if (input.Price != default)
                existing.Price = input.Price;

            _productService.Update(existing);
            return Ok();
        }

        // DELETE /api/products/{id} - deletes via string path param
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = _productService.GetById(int.Parse(id));
            if (product is null) return NotFound();

            _productService.Delete(int.Parse(id));
            return Ok();
        }
    }
}
