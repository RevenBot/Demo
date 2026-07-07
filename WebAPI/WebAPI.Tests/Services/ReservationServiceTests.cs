using MongoDB.Bson;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Services;
using Xunit;

namespace WebAPI.Tests.Services;

public class ReservationServiceTests : IDisposable
{
    private readonly string _dbName = $"TestResDb_{Guid.NewGuid():N}";
    private RestaurantReservationDbContext? _context;

    public ReservationServiceTests()
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

    private ReservationService CreateService(RestaurantReservationDbContext? ctx = null) =>
        new(ctx ?? _context!);

    [Fact]
    public async Task AddReservation_ShouldInsertIntoDatabase()
    {
        // Arrange - add a restaurant first so the lookup in service succeeds
        var restaurant = new Restaurant 
        { 
            Name = "Test Restaurant", 
            Cuisine = "Italian",
            Borough = "Manhattan"
        };
        var rest = _context!.Restaurants.Add(restaurant);
        var idRestaurant = rest.Entity.Id;
        await _context.SaveChangesAsync();

        var reservation = new Reservation
        {
            RestaurantId = idRestaurant
        };

        
        // Insert a dummy first to get an auto-generated id, then re-create properly
        var res = await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        var resEntity = res.Entity;

        // Assert - Id should have been generated (24-char hex ObjectId)
        Assert.NotEmpty(resEntity.Id);
        Assert.NotEmpty(resEntity.RestaurantId);
        Assert.Equal(idRestaurant, resEntity.RestaurantId);
        Assert.Equal(24, resEntity.Id.Length);
        Assert.True(IsHexString(resEntity.Id));

        // Also test service.AddReservation return value (returns the Reservation entity)
        ReloadContext();
        var service = CreateService(_context!);
        var newRes = new Reservation { RestaurantId = idRestaurant };
        var added = service.AddReservation(newRes);
        Assert.NotNull(added);
        Assert.NotEmpty(added.Id);
    }


    [Fact]
    public async Task GetAllReservations_ShouldReturnPagedResult()
    {
        // Arrange - add restaurants first for the lookup
        var restaurant1 = new Restaurant { Name = "Restaurant 1", Cuisine = "Italian", Borough = "Manhattan" };
        var restaurant2 = new Restaurant { Name = "Restaurant 2", Cuisine = "Japanese", Borough = "Queens" };

        _context!.Restaurants.AddRange(restaurant1, restaurant2);
        await _context.SaveChangesAsync();

        var res1 = new Reservation 
        { 
            Date = new DateTime(2025, 6, 1, 19, 0, 0),
            RestaurantId = ObjectId.GenerateNewId().ToString() 
        };
        var res2 = new Reservation 
        { 
            Date = new DateTime(2024, 6, 1, 19, 0, 0),
            RestaurantId = ObjectId.GenerateNewId().ToString() 
        };

        await _context.Reservations.AddRangeAsync(res1, res2);
        await _context.SaveChangesAsync();

        // Act - reload context to see persisted data (InMemory needs this)
        ReloadContext();
        var service = CreateService(_context!);
        var result = service.GetAllReservations(0, 10);
        var reservations = result.Items.ToList();

        // Assert - PagedResult structure and count (service does NOT order by date)
        Assert.Equal(2, reservations.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetReservationById_ShouldReturn_WhenExists()
    {
        // Arrange
        var restaurant = new Restaurant { Name = "Test", Cuisine = "Italian", Borough = "Manhattan" };
        _context!.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        var reservation = new Reservation 
        { 
            Date = DateTime.Now,
            RestaurantId = ObjectId.GenerateNewId().ToString() 
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
        
        ReloadContext();
        var service = CreateService(_context!);

        // Act
        var result = service.GetReservationById(reservation.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(reservation.Date, result!.Date);
    }

    [Fact]
    public void GetReservationById_ShouldReturnNull_WhenDoesNotExist()
    {
        // Arrange - no reservations exist
        ReloadContext();
        var service = CreateService(_context!);

        // Act
        var result = service.GetReservationById("000000000000000000000000");

        // Assert
        Assert.Null(result);
    }
    [Fact]
    public async Task EditReservation_ShouldUpdateDate()
    {
        // Arrange
        var restaurant = new Restaurant { Name = "Test", Cuisine = "Italian", Borough = "Manhattan" };
        _context!.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        var originalDate = new DateTime(2025, 1, 1, 19, 0, 0);
        var reservation = new Reservation 
        { 
            Date = originalDate,
            RestaurantId = ObjectId.GenerateNewId().ToString() 
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        ReloadContext();
        var service = CreateService(_context!);
        var updated = _context!.Reservations.First(r => r.Id == reservation.Id);
        
        var newDate = new DateTime(2025, 6, 15, 20, 30, 0);

        // Act
        updated.Date = newDate;
        var edited = service.EditReservation(updated);

        // Assert - need to reload again since InMemory doesn't auto-refresh context
        ReloadContext();
        var persisted = _context!.Reservations.First(r => r.Id == reservation.Id);
        Assert.Equal(newDate, persisted.Date);

        // Assert - EditReservation returns the edited entity
        Assert.NotNull(edited);
        Assert.Equal(newDate, edited.Date);
    }
    [Fact]
    public void EditReservation_NonExistent_ShouldThrow()
    {
        // Arrange - no reservations exist
        ReloadContext();
        var service = CreateService(_context!);
        
        var fakeRes = new Reservation 
        { 
            Date = DateTime.Now, 
            Id = "000000000000000000000002" 
        };

        // Act & Assert
        var ex = Record.Exception(() => service.EditReservation(fakeRes));
        Assert.NotNull(ex);
    }

    [Fact]
    public async Task DeleteReservation_ShouldRemoveFromDatabase()
    {
        // Arrange
        var restaurant = new Restaurant { Name = "Test", Cuisine = "Italian", Borough = "Manhattan" };
        _context!.Restaurants.Add(restaurant);
        await _context.SaveChangesAsync();

        var reservation = new Reservation 
        { 
            Date = DateTime.Now,
            RestaurantId = ObjectId.GenerateNewId().ToString() 
        };
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();

        ReloadContext();
        var service = CreateService(_context!);

        // Act - need to re-fetch after reload for the delete call
        var existing = _context!.Reservations.First(r => r.Id == reservation.Id);
        service.DeleteReservation(existing);

        // Assert
        var deleted = _context.Reservations.Find(reservation.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public void DeleteReservation_NonExistent_ShouldThrow()
    {
        // Arrange - no reservations exist, so none can be found
        ReloadContext();
        var service = CreateService(_context!);
        
        var fakeRes = new Reservation 
        { 
            Date = DateTime.Now, 
            Id = "000000000000000000000001" 
        };

        // Act & Assert
        var ex = Record.Exception(() => service.DeleteReservation(fakeRes));
        Assert.NotNull(ex);
    }



    private static bool IsHexString(string s)
    {
        foreach (char c in s)
            if (!char.IsAsciiHexDigit(c)) return false;
        return true;
    }
}
