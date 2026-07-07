using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WebAPI.Services;
using WebAPI.Models;
using WebAPI.Dto;
using Moq;
using Xunit;

namespace WebAPI.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductsController(_mockService.Object);
    }

    [Fact]
    public async Task GetProducts_ShouldReturnOkResultWithDtos()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 999m },
            new Product { Id = 2, Name = "Mouse", Price = 25m }
        };
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new PagedResult<Product>
        {
            Items = products,
            TotalCount = 2,
            PageSize = 10,
            CurrentPage = 1
        }));

        // Act
        var result = await _controller.GetProducts(new PaginationParamsDto());

        // Assert
        Assert.NotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var pagedResult = okResult!.Value as PagedResult<ProductOutputDto>;
        Assert.NotNull(pagedResult);
        Assert.Equal(2, pagedResult.TotalCount);
        Assert.Equal(10, pagedResult.PageSize);
        Assert.Equal(2, pagedResult.Items.Count());
        Assert.Equal("1", pagedResult.Items.First().Id);
        Assert.Equal("Laptop", pagedResult.Items.First().Name);
    }
    [Fact]
    public async Task GetProducts_ShouldReturnEmptyList_WhenNoProducts()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new PagedResult<Product>
        {
            Items = new List<Product>(),
            TotalCount = 0,
            PageSize = 10,
            CurrentPage = 1
        }));

        // Act
        var result = await _controller.GetProducts(new PaginationParamsDto());

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var pagedResult = okResult!.Value as PagedResult<ProductOutputDto>;
        Assert.NotNull(pagedResult);
        Assert.Equal(0, pagedResult.TotalCount);
        Assert.Empty(pagedResult.Items.ToList());
    }

    [Fact]
    public async Task GetProduct_WhenExists_ShouldReturnOkWithDto()
    {
        // Arrange
        var product = new Product { Id = 42, Name = "Tablet", Price = 500m };
        _mockService.Setup(s => s.GetByIdAsync(42)).Returns(Task.FromResult<Product?>(product));

        // Act
        var result = await _controller.GetProduct("42");

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        
        var dto = okResult!.Value as ProductOutputDto;
        Assert.NotNull(dto);
        Assert.Equal("42", dto.Id);
        Assert.Equal("Tablet", dto.Name);
        Assert.Equal(500m, dto.Price);
    }

    [Fact]
    public async Task GetProduct_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(99)).Returns(Task.FromResult((Product?)null));

        // Act
        var result = await _controller.GetProduct("99");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostProduct_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var inputDto = new ProductInputDto { Name = "New Chair", Price = 150m };
        var addedProduct = new Product { Id = 1, Name = "New Chair", Price = 150m };
        _mockService.Setup(s => s.AddAsync(It.IsAny<Product>())).Returns(Task.FromResult(addedProduct));

        // Act
        var result = await _controller.PostProduct(inputDto);

        // Assert
        Assert.NotNull(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdResult);
        
        var dto = createdResult.Value as ProductOutputDto;
        Assert.NotNull(dto);

        Assert.Equal("1", dto.Id);
        Assert.Equal("New Chair", dto.Name);
        Assert.Equal(150m, dto.Price);
    }

    [Fact]
    public async Task PutProduct_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(5)).Returns(Task.FromResult((Product?)null));

        var input = new ProductInputDto 
        { 
            Name = "Updated", 
            Price = 30m 
        };

        // Act
        var result = await _controller.PutProduct("5", input);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PutProduct_WhenExists_ShouldUpdateAndReturnOk()
    {
        // Arrange
        var existing = new Product 
        { 
            Id = 1, 
            Name = "Old", 
            Price = 10m 
        };
        _mockService.Setup(s => s.GetByIdAsync(1)).Returns(Task.FromResult<Product?>(existing));
        _mockService.Setup(s => s.UpdateAsync(It.IsAny<Product>())).Returns(Task.FromResult(existing));

        var input = new ProductInputDto 
        { 
            Name = "Updated", 
            Price = 20m 
        };

        // Act
        var result = await _controller.PutProduct("1", input);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        
        var dto = okResult.Value as ProductOutputDto;
        Assert.NotNull(dto);
        Assert.Equal("1", dto.Id);
        Assert.Equal("Updated", dto.Name);
        
        // Verify Update was called
        _mockService.Verify(s => s.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetByIdAsync(7)).Returns(Task.FromResult((Product?)null));

        // Act
        var result = await _controller.DeleteProduct("7");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_WhenExists_ShouldDeleteAndReturnOk()
    {
        // Arrange
        var product = new Product 
        { 
            Id = 3, 
            Name = "Will Delete", 
            Price = 5m 
        };
        _mockService.Setup(s => s.GetByIdAsync(3)).Returns(Task.FromResult<Product?>(product));

        // Act
        var result = await _controller.DeleteProduct("3");

        // Assert
        Assert.IsType<OkResult>(result);
        
        // Verify Delete was called with correct ID
        _mockService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Once);
    }

}