using WebAPI.Dto;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace WebAPI.Tests.Dtos;

/// <summary>
/// Tests for input DTOs - validates DataAnnotations work as expected.
/// </summary>
public class ProductInputDtoTests
{
    [Fact]
    public void Validate_ValidProduct_ShouldReturnSuccess()
    {
        // Arrange
        var dto = new ProductInputDto 
        { 
            Name = "Valid Product", 
            Price = 25m 
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
        var dto = new ProductInputDto 
        { 
            Name = "", 
            Price = 25m 
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
    public void Validate_ZeroPrice_ShouldPass_WhenNameIsProvided()
    {
        // Arrange - Price of 0 should be valid because [Required] on decimal only rejects null, not 0
        var dto = new ProductInputDto 
        { 
            Name = "Free Item",
            Price = 0m 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert - should pass because Required on value types only checks for null (impossible here)
        Assert.True(isValid);
        Assert.Empty(results);
    }
}
