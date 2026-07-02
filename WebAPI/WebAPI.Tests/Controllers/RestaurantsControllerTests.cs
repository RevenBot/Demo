using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WebAPI.Services;
using WebAPI.Models;
using WebAPI.Dto;
using Moq;
using Xunit;

namespace WebAPI.Tests.Controllers;

public class RestaurantsControllerTests
{
    private readonly Mock<IRestaurantService> _mockService;
    private readonly RestaurantsController _controller;

    public RestaurantsControllerTests()
    {
        _mockService = new Mock<IRestaurantService>();
        _controller = new RestaurantsController(_mockService.Object);
    }

    [Fact]
    public async Task GetRestaurants_ShouldReturnOkResultWithDtos()
    {
        // Arrange
        var restaurants = new List<Restaurant>
        {
            new Restaurant { Id = "60d5ec49f1b2c8b4e8f1a1a1", Name = "Italian Bistro", Cuisine = "Italian", Borough = "Manhattan" },
            new Restaurant { Id = "60d5ec49f1b2c8b4e8f1a1a2", Name = "Sushi Place", Cuisine = "Japanese", Borough = "Queens" }
        };
        _mockService.Setup(s => s.GetAllRestaurants()).Returns(restaurants.AsEnumerable);

        // Act
        var result = await _controller.GetRestaurants();

        // Assert
        Assert.NotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        
        var dtos = okResult!.Value as IEnumerable<RestaurantOutputDto>;
        Assert.NotNull(dtos);
        var dtoList = dtos.ToList();
        Assert.Equal(2, dtoList.Count);
        Assert.Equal("60d5ec49f1b2c8b4e8f1a1a1", dtoList[0].Id);
        Assert.Equal("Italian Bistro", dtoList[0].Name);
    }

    [Fact]
    public async Task GetRestaurants_ShouldReturnEmptyList_WhenNoRestaurants()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllRestaurants()).Returns(new List<Restaurant>().AsEnumerable);

        // Act
        var result = await _controller.GetRestaurants();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        
        var dtos = okResult!.Value as IEnumerable<RestaurantOutputDto>;
        Assert.NotNull(dtos);
        Assert.Empty(dtos!);
    }

    [Fact]
    public async Task GetRestaurant_WhenExists_ShouldReturnOkWithDto()
    {
        // Arrange
        var restaurant = new Restaurant 
        { 
            Id = "60d5ec49f1b2c8b4e8f1a1a3", 
            Name = "Thai Garden", 
            Cuisine = "Thai", 
            Borough = "Brooklyn" 
        };
        _mockService.Setup(s => s.GetRestaurantById("60d5ec49f1b2c8b4e8f1a1a3")).Returns(restaurant);

        // Act
        var result = await _controller.GetRestaurant("60d5ec49f1b2c8b4e8f1a1a3");

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);
        
        var dto = okResult!.Value as RestaurantOutputDto;
        Assert.NotNull(dto);
        Assert.Equal("60d5ec49f1b2c8b4e8f1a1a3", dto.Id);
        Assert.Equal("Thai Garden", dto.Name);
        Assert.Equal("Thai", dto.Cuisine);
        Assert.Equal("Brooklyn", dto.Borough);
    }

    [Fact]
    public async Task GetRestaurant_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetRestaurantById("nonexistent")).Returns((Restaurant?)null);

        // Act
        var result = await _controller.GetRestaurant("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostRestaurant_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var inputDto = new RestaurantInputDto 
        { 
            Name = "New Cafe", 
            Cuisine = "American", 
            Borough = "Bronx" 
        };
        
        string? capturedId = null;
        _mockService.Setup(s => s.AddRestaurant(It.IsAny<Restaurant>()))
                    .Callback<Restaurant>(r => capturedId = r.Id);

        // Act
        var result = await _controller.PostRestaurant(inputDto);

        // Assert
        Assert.NotNull(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdResult);
        
        var dto = createdResult.Value as RestaurantOutputDto;
        Assert.NotNull(dto);

        Assert.Equal(capturedId, dto.Id);
    }

    [Fact]
    public async Task PutRestaurant_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetRestaurantById("5")).Returns((Restaurant?)null);

        var input = new RestaurantInputDto 
        { 
            Name = "Updated", 
            Cuisine = "French", 
            Borough = "Manhattan" 
        };

        // Act
        var result = await _controller.UpdateRestaurant("5", input);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutRestaurant_WhenExists_ShouldUpdateAndReturnOk()
    {
        // Arrange
        var existing = new Restaurant 
        { 
            Id = "60d5ec49f1b2c8b4e8f1a1a4", 
            Name = "Old Name", 
            Cuisine = "Mexican", 
            Borough = "Queens" 
        };
        _mockService.Setup(s => s.GetRestaurantById("60d5ec49f1b2c8b4e8f1a1a4")).Returns(existing);

        var input = new RestaurantInputDto 
        { 
            Name = "Updated", 
            Cuisine = "Italian", 
            Borough = "Manhattan" 
        };

        // Act
        var result = await _controller.UpdateRestaurant("60d5ec49f1b2c8b4e8f1a1a4", input);

        // Assert
        Assert.IsType<OkResult>(result);
        
        // Verify Edit was called
        _mockService.Verify(s => s.EditRestaurant(It.IsAny<Restaurant>()), Times.Once);
    }

    [Fact]
    public async Task DeleteReservation_WhenNotFound_ShouldReturnNotFound() {
        // Arrange
        _mockService.Setup(s => s.GetRestaurantById("60d5ec49f1b2c8b4e8f1a1a5")).Returns((Restaurant?)null);

        // Act
        var result = await _controller.DeleteRestaurant("60d5ec49f1b2c8b4e8f1a1a5");

        // Assert
        Assert.IsType<NotFoundResult>(result);

    }
    [Fact]
    public async Task DeleteRestaurant_WhenExists_ShouldDeleteAndReturnOk()
    {
        // Arrange
        var reservation = new Restaurant
        {
            Id = "60d5ec49f1b2c8b4e8f1a1a5",
            Borough = "TestToDelete",
            Cuisine = "TestToDelete",
            Name = "TestToDelete"
        };
        _mockService.Setup(s => s.GetRestaurantById("60d5ec49f1b2c8b4e8f1a1a5")).Returns(reservation);

        // Act
        var result = await _controller.DeleteRestaurant("60d5ec49f1b2c8b4e8f1a1a5");

        // Assert
        Assert.IsType<OkResult>(result);

        // Verify Delete was called
        _mockService.Verify(s => s.DeleteRestaurant(It.IsAny<Restaurant>()), Times.Once);
    }
}
