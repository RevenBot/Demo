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
        public async Task<ActionResult<PagedResult<ProductOutputDto>>> GetProducts([FromQuery] PaginationParamsDto paramsDto)
        {
            int skip = (paramsDto.PageNumber - 1) * paramsDto.PageSize;

            var pagedResult = _productService.GetAll(skip, paramsDto.PageSize);

            return Ok(new PagedResult<ProductOutputDto>
            {
                Items = pagedResult.Items.Select(p => new ProductOutputDto
                {
                    Id = p.Id.ToString(),
                    Name = p.Name,
                    Price = p.Price
                }).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageSize = pagedResult.PageSize,
                CurrentPage = pagedResult.CurrentPage
            });
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

            var added = _productService.Add(product);

            return CreatedAtAction("GetProduct", 
                new { id = added.Id }, 
                new ProductOutputDto
                {
                    Id = added.Id.ToString(),
                    Name = added.Name,
                    Price = added.Price
                });
        }

        // PUT /api/products/{id} - updates via string path param (no id in body)
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductOutputDto>> PutProduct(string id, [FromBody] ProductInputDto input)
        {
            var existing = _productService.GetById(int.Parse(id));
            if (existing is null) return NotFound();

            // Only update fields that were actually provided in the body
            if (!string.IsNullOrEmpty(input.Name))
                existing.Name = input.Name;
            existing.Price = input.Price;

            var updated = _productService.Update(existing);

            return Ok(new ProductOutputDto
            {
                Id = updated.Id.ToString(),
                Name = updated.Name,
                Price = updated.Price
            });
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
