using WebAPI.Models;
using MongoDB.Bson;
using Xunit;

namespace WebAPI.Tests.Services;

/// <summary>
/// Tests that verify model validation and data integrity 
/// without requiring a live MongoDB connection.
/// </summary>
public class ModelValidationTests
{
    [Fact]
    public void NewRestaurant_ShouldHaveObjectIdDefault()
    {
        // Arrange & Act
        var restaurant = new Restaurant();
        // Assert - ObjectId auto-generation produces a 24-char hex string
        Assert.NotNull(restaurant.Id);
        Assert.NotEmpty(restaurant.Id);
        Assert.Equal(24, restaurant.Id.Length);
        Assert.True(IsHexString(restaurant.Id));
    }

    [Fact]
    public void NewReservation_ShouldHaveObjectIdDefault()
    {
        // Arrange & Act
        var reservation = new Reservation();

        // Assert
        Assert.NotNull(reservation.Id);
        Assert.NotEmpty(reservation.Id);
        Assert.Equal(24, reservation.Id.Length);
        Assert.True(IsHexString(reservation.Id));
        
        // RestaurantId is nullable by design
        Assert.Null(reservation.RestaurantId);
    }

    [Fact]
    public void NewProduct_ShouldHaveValidDefaults()
    {
        var product = new Product();
        
        Assert.Equal(0, product.Id);          // Default for int
        Assert.True(string.IsNullOrEmpty(product.Name)); // string.Empty IS null-or-empty
        Assert.Equal(0m, product.Price);      // Decimal default
        
        // Name defaults to string.Empty (per Product.cs = string.Empty)
        Assert.Equal(string.Empty, product.Name);
    }

    [Fact]
    public void Product_ShouldAcceptValidData()
    {
        var product = new Product 
        { 
            Id = 1,
            Name = "Coffee Mug", 
            Price = 12.5m 
        };

        Assert.Equal(1, product.Id);
        Assert.Equal("Coffee Mug", product.Name);
        Assert.Equal(12.5m, product.Price);
    }

    [Fact]
    public void Reservation_ShouldAcceptFullData()
    {
        var validDate = new DateTime(2025, 12, 25, 19, 0, 0);
        var reservation = new Reservation
        {
            RestaurantId = ObjectId.GenerateNewId().ToString(),
            RestaurantName = "Grand Dining",
            Date = validDate
        };

        Assert.NotNull(reservation.RestaurantId);
        Assert.Equal("Grand Dining", reservation.RestaurantName);
        Assert.Equal(validDate, reservation.Date);
    }

    [Fact]
    public void ProductPrice_CanBeSetToZero()
    {
        var product = new Product 
        { 
            Id = 10,
            Name = "Free Item", 
            Price = 0m 
        };

        Assert.Equal(0m, product.Price);
    }

    [Fact]
    public void RestaurantId_IsValidObjectIdFormat()
    {
        var reservation = new Reservation();
        var objectId = ObjectId.GenerateNewId().ToString();
        
        // Verify the generated string is valid for MongoDB ObjectId reference
        Assert.Equal(24, objectId.Length);
        Assert.True(IsHexString(objectId));
        reservation.RestaurantId = objectId;
    }

    private static bool IsHexString(string s)
    {
        foreach (char c in s)
        {
            if (!char.IsAsciiHexDigit(c)) return false;
        }
        return true;
    }
}