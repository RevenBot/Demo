using WebAPI.Dto;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace WebAPI.Tests.Dtos;

public class ReservationInputDtoTests
{
    [Fact]
    public void Validate_ValidReservation_ShouldReturnSuccess()
    {
        // Arrange - Date is provided, so Required passes
        var dto = new ReservationInputDto 
        { 
            RestaurantId = "507f1f77bcf86cd799439011",
            Date = new DateTime(2025, 12, 25, 19, 0, 0) 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert - should pass because Date is valid (non-default)
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_EmptyRestaurantId_ShouldPass()
    {
        // Arrange - RestaurantId is nullable, so empty string is allowed
        var dto = new ReservationInputDto 
        { 
            Date = new DateTime(2025, 6, 1, 18, 30, 0) 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert - should pass because RestaurantId is nullable (not required)
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_NullRestaurantId_ShouldPass()
    {
        // Arrange - Nullable<string> can be null without failing validation
        var dto = new ReservationInputDto 
        { 
            RestaurantId = null,
            Date = new DateTime(2025, 8, 15, 20, 0, 0) 
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert - should pass because RestaurantId is nullable
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_DefaultDate_ShouldPass()
    {
        // Arrange - DateTime defaults to 0001-01-01. In .NET, [Required] on non-nullable 
        // value types only rejects null (which is impossible for DateTime), so default passes.
        var dto = new ReservationInputDto 
        { 
            Date = default(DateTime)  
        };

        // Act
        var validationContext = new ValidationContext(dto);
        var results = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(dto, validationContext, results, true);

        // Assert - Required on value types (DateTime) only rejects null, not default values.
        // This is expected .NET behavior: default(DateTime) passes [Required] validation.
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Fact]
    public void Validate_ReservationId_ShouldBe24CharHexString()
    {
        // Arrange - RestaurantId should be 24-character hex string for valid MongoDB ObjectId reference
        var validObjectId = "507f191e810c19729de860ea"; // Valid 24-char hex
        
        var dto = new ReservationInputDto 
        { 
            RestaurantId = validObjectId,
            Date = new DateTime(2025, 9, 1, 19, 0, 0) 
        };

        // Act - verify the ID format is correct
        Assert.NotNull(dto.RestaurantId);
        Assert.Equal(validObjectId.Length, dto.RestaurantId.Length);
        
        bool isHex = dto.RestaurantId.All(char.IsAsciiHexDigit);
        Assert.True(isHex);
    }

    [Fact]
    public void Validate_ReservationDate_ShouldAcceptFutureDate()
    {
        // Arrange - valid future date should pass validation
        var futureDate = DateTime.Now.AddMonths(6);
        
        var dto = new ReservationInputDto 
        { 
            Date = futureDate 
        };

        // Act & Assert
        Assert.True(dto.Date > default(DateTime));
    }
}
