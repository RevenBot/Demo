using WebAPI.Dto;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace WebAPI.Tests.Dtos;

/// <summary>
/// Tests for Restaurant input DTO - validates DataAnnotations work as expected.
/// </summary>
public class RestaurantInputDtoTests
{
    [Fact]
    public void Validate_ValidRestaurant_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new RestaurantInputDto 
        { 
            Name = "Valid Restaurant", 
            Cuisine = "Italian", 
            Borough = "Manhattan" 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_EmptyName_ShouldFail()
    {
        // Arrange
        var dto = new RestaurantInputDto 
        { 
            Name = "", 
            Cuisine = "Italian", 
            Borough = "Manhattan" 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        
        var nameResults = results.Where(r => r.MemberNames.Contains("Name")).ToList();
        Assert.NotEmpty(nameResults);
    }

    [Fact]
    public void Validate_EmptyCuisine_ShouldFail()
    {
        // Arrange
        var dto = new RestaurantInputDto 
        { 
            Name = "Valid Restaurant", 
            Cuisine = "", 
            Borough = "Manhattan" 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        
        var cuisineResults = results.Where(r => r.MemberNames.Contains("Cuisine")).ToList();
        Assert.NotEmpty(cuisineResults);
    }

    [Fact]
    public void Validate_EmptyBorough_ShouldFail()
    {
        // Arrange
        var dto = new RestaurantInputDto 
        { 
            Name = "Valid Restaurant", 
            Cuisine = "Italian", 
            Borough = "" 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert
        Assert.False(isValid);
        Assert.NotEmpty(results);
        
        var boroughResults = results.Where(r => r.MemberNames.Contains("Borough")).ToList();
        Assert.NotEmpty(boroughResults);
    }

    [Fact]
    public void Validate_AllFieldsProvided_ShouldPass()
    {
        // Arrange
        var dto = new RestaurantInputDto 
        { 
            Name = "Tokyo Kitchen", 
            Cuisine = "Japanese", 
            Borough = "Queens" 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}
