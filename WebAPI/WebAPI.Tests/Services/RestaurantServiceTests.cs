using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Services;
using MongoDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebAPI.Tests.Services;

public class RestaurantServiceTests : IDisposable
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";
    private RestaurantReservationDbContext? _context;

    public RestaurantServiceTests()
    {
        var options = new DbContextOptionsBuilder<RestaurantReservationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
        _context = new RestaurantReservationDbContext(options);
    }

    private void ReloadContext()
    {
        _context!.Dispose();
        var options = new DbContextOptionsBuilder<RestaurantReservationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
        _context = new RestaurantReservationDbContext(options);
    }

    public void Dispose()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
    }

    [Fact]
    public async Task Add_Restaurant_ShouldInsertIntoDatabase()
    {
        // Arrange
        var service = new RestaurantService(_context!);
        var restaurant = new Restaurant 
        { 
            Name = "Test Restaurant", 
            Cuisine = "Italian", 
            Borough = "Manhattan" 
        };

        // Act
        service.AddRestaurant(restaurant);
        
        // Assert
        var saved = await _context!.Restaurants.FindAsync(restaurant.Id);
        Assert.NotNull(saved);
        Assert.Equal("Test Restaurant", saved!.Name);
        Assert.Equal("Italian", saved.Cuisine);
    }

    [Fact]
    public async Task GetAllRestaurants_ShouldReturnAll()
    {
        // Arrange
        await _context!.Restaurants.AddRangeAsync(
            new Restaurant { Name = "Restaurant A", Cuisine = "Italian", Borough = "Manhattan" },
            new Restaurant { Name = "Restaurant B", Cuisine = "Japanese", Borough = "Queens" }
        );
        await _context.SaveChangesAsync();

        var service = new RestaurantService(_context!);

        // Act
        var restaurants = service.GetAllRestaurants().ToList();

        // Assert
        Assert.Equal(2, restaurants.Count);
    }

    [Fact]
    public void GetRestaurantById_ShouldReturnRestaurant_WhenExists()
    {
        // Arrange
        var expected = new Restaurant 
        { 
            Name = "Found Restaurant", 
            Cuisine = "Thai", 
            Borough = "Brooklyn" 
        };
        _context!.Restaurants.Add(expected);
        _context.SaveChanges();
        ReloadContext();

        var service = new RestaurantService(_context!);

        // Act
        var result = service.GetRestaurantById(expected.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Found Restaurant", result!.Name);
    }

    [Fact]
    public void GetRestaurantById_ShouldReturnNull_WhenDoesNotExist()
    {
        // Arrange
        ReloadContext();
        var service = new RestaurantService(_context!);

        // Act
        var result = service.GetRestaurantById("nonexistent");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void EditRestaurant_ShouldModifyExisting()
    {
        // Arrange
        var restaurant = new Restaurant 
        { 
            Name = "Old Name", 
            Cuisine = "Mexican", 
            Borough = "Bronx" 
        };
        _context!.Restaurants.Add(restaurant);
        _context.SaveChanges();
        ReloadContext();

        var service = new RestaurantService(_context!);
        
        // Act
        restaurant.Name = "New Name";
        restaurant.Cuisine = "French";
        restaurant.Borough = "Manhattan";
        service.EditRestaurant(restaurant);

        // Assert
        var updated = _context!.Restaurants.Find(restaurant.Id);
        Assert.NotNull(updated);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal("French", updated.Cuisine);
    }
    [Fact]
    public void EditRestaurant_NonExistent_ShouldThrow()
    {
        // Arrange - no reservations exist
        ReloadContext();
        var service = new RestaurantService(_context!);
        
        var fakeRes = new Restaurant 
        { 
            Id = "000000000000000000000002" ,
            Name = "Old Name", 
            Cuisine = "Mexican", 
            Borough = "Bronx" 
        };

        // Act & Assert
        var ex = Record.Exception(() => service.EditRestaurant(fakeRes));
        Assert.NotNull(ex);
    }

    [Fact]
    public void Delete_Restaurant_ShouldRemoveFromDatabase()
    {
        // Arrange
        var restaurant = new Restaurant 
        { 
            Name = "To Delete", 
            Cuisine = "Chinese", 
            Borough = "Flushing" 
        };
        _context!.Restaurants.Add(restaurant);
        _context.SaveChanges();
        ReloadContext();

        var service = new RestaurantService(_context!);

        // Act
        service.DeleteRestaurant(restaurant);

        // Assert
        var deleted = _context!.Restaurants.Find(restaurant.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public void Delete_NonExistent_ShouldThrow()
    {
        // Arrange
        var fakeRestaurant = new Restaurant 
        { 
            Id = "nonexistent_id", 
            Name = "Does Not Exist", 
            Cuisine = "", 
            Borough = "" 
        };

        ReloadContext();
        var service = new RestaurantService(_context!);

        // Act & Assert - should throw because DB won't have the restaurant
        var ex = Record.Exception(() => service.DeleteRestaurant(fakeRestaurant));
        
        // The service throws ArgumentException when restaurant not found
        Assert.NotNull(ex);
    }
}
