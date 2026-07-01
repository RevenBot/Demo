using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using WebAPI.Models;
using WebAPI.Services;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        return Ok(_productService.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _productService.GetById(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        _productService.Add(product);
        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, Product product)
    {
        if (id != product.Id)
            return BadRequest();
        _productService.Update(product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var product = _productService.GetById(id);
        if (product == null)
            return NotFound();
        _productService.Delete(id);
        return NoContent();
    }
}
