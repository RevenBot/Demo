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
        _mockService.Setup(s => s.GetAll()).Returns(products.AsEnumerable);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        Assert.NotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        
        var dtos = okResult!.Value as IEnumerable<ProductOutputDto>;
        Assert.NotNull(dtos);
        var dtoList = dtos.ToList();
        Assert.Equal(2, dtoList.Count);
        Assert.Equal("1", dtoList[0].Id);
        Assert.Equal("Laptop", dtoList[0].Name);
    }
    [Fact]
    public async Task GetProducts_ShouldReturnEmptyList_WhenNoProducts()
    {
        // Arrange
        _mockService.Setup(s => s.GetAll()).Returns(new List<Product>().AsEnumerable);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        
        var dtos = okResult!.Value as IEnumerable<ProductOutputDto>;
        Assert.NotNull(dtos);
        Assert.Empty(dtos!);
    }

    [Fact]
    public async Task GetProduct_WhenExists_ShouldReturnOkWithDto()
    {
        // Arrange
        var product = new Product { Id = 42, Name = "Tablet", Price = 500m };
        _mockService.Setup(s => s.GetById(42)).Returns(product);

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
        _mockService.Setup(s => s.GetById(99)).Returns((Product?)null);

        // Act
        var result = await _controller.GetProduct("99");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostProduct_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var inputDto = new ProductInputDto 
        { 
            Name = "New Chair", 
            Price = 150m 
        };
        
        string? capturedId = null;
        _mockService.Setup(s => s.Add(It.IsAny<Product>()))
                    .Callback<Product>(p => capturedId = p.Id.ToString());

        // Act
        var result = await _controller.PostProduct(inputDto);

        // Assert
        Assert.NotNull(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdResult);
        
        var dto = createdResult.Value as ProductOutputDto;
        Assert.NotNull(dto);

        Assert.Equal(capturedId, dto.Id);
        Assert.Equal(inputDto.Name, dto.Name);
        Assert.Equal(inputDto.Price, dto.Price);
    }

    [Fact]
    public async Task PutProduct_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetById(5)).Returns((Product?)null);

        var input = new ProductInputDto 
        { 
            Name = "Updated", 
            Price = 30m 
        };

        // Act
        var result = await _controller.PutProduct("5", input);

        // Assert
        Assert.IsType<NotFoundResult>(result);
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
        _mockService.Setup(s => s.GetById(1)).Returns(existing);

        var input = new ProductInputDto 
        { 
            Name = "Updated", 
            Price = 20m 
        };

        // Act
        var result = await _controller.PutProduct("1", input);

        // Assert
        Assert.IsType<OkResult>(result);
        
        // Verify Update was called
        _mockService.Verify(s => s.Update(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetById(7)).Returns((Product?)null);

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
        _mockService.Setup(s => s.GetById(3)).Returns(product);

        // Act
        var result = await _controller.DeleteProduct("3");

        // Assert
        Assert.IsType<OkResult>(result);
        
        // Verify Delete was called with correct ID
        _mockService.Verify(s => s.Delete(It.IsAny<int>()), Times.Once);
    }

}