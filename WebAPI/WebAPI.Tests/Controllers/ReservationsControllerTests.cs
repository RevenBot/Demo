using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers;
using WebAPI.Services;
using WebAPI.Models;
using WebAPI.Dto;
using Moq;
using Xunit;

namespace WebAPI.Tests.Controllers;

public class ReservationsControllerTests
{
    private readonly Mock<IReservationService> _mockService;
    private readonly ReservationsController _controller;

    public ReservationsControllerTests()
    {
        _mockService = new Mock<IReservationService>();
        _controller = new ReservationsController(_mockService.Object);
    }

    [Fact]
    public async Task GetReservations_ShouldReturnOkResultWithDtos()
    {
        // Arrange
        var reservations = new List<Reservation>
        {
            new Reservation
            {
                Id = "60d5ec49f1b2c8b4e8f1a1a1",
                RestaurantId = "60d5ec49f1b2c8b4e8f1a1a2",
                RestaurantName = "Italian Bistro",
                Date = DateTime.Parse("2024-07-22T19:00:00")
            },
            new Reservation
            {
                Id = "60d5ec49f1b2c8b4e8f1a1a3",
                RestaurantId = "60d5ec49f1b2c8b4e8f1a1a4",
                RestaurantName = "Sushi Place",
                Date = DateTime.Parse("2024-07-23T20:00:00")
            }
        };
        _mockService.Setup(s => s.GetAllReservations()).Returns(reservations.AsEnumerable);

        // Act
        var result = await _controller.GetReservations();

        // Assert
        Assert.NotNull(result.Result);
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var dtos = okResult!.Value as IEnumerable<ReservationOutputDto>;
        Assert.NotNull(dtos);
        var dtoList = dtos.ToList();
        Assert.Equal(2, dtoList.Count);
        Assert.Equal("60d5ec49f1b2c8b4e8f1a1a1", dtoList[0].Id);
        Assert.Equal("Italian Bistro", dtoList[0].RestaurantName);
    }

    [Fact]
    public async Task GetReservations_ShouldReturnEmptyList_WhenNoReservations()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllReservations()).Returns(new List<Reservation>().AsEnumerable);

        // Act
        var result = await _controller.GetReservations();

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var dtos = okResult!.Value as IEnumerable<ReservationOutputDto>;
        Assert.NotNull(dtos);
        Assert.Empty(dtos!);
    }

    [Fact]
    public async Task GetReservation_WhenExists_ShouldReturnOkWithDto()
    {
        // Arrange
        var reservation = new Reservation
        {
            Id = "60d5ec49f1b2c8b4e8f1a1a3",
            RestaurantId = "60d5ec49f1b2c8b4e8f1a1a4",
            RestaurantName = "Thai Garden",
            Date = DateTime.Parse("2024-07-25T19:30:00")
        };
        _mockService.Setup(s => s.GetReservationById("60d5ec49f1b2c8b4e8f1a1a3")).Returns(reservation);

        // Act
        var result = await _controller.GetReservation("60d5ec49f1b2c8b4e8f1a1a3");

        // Assert
        var okResult = result.Result as OkObjectResult;
        Assert.NotNull(okResult);

        var dto = okResult!.Value as ReservationOutputDto;
        Assert.NotNull(dto);
        Assert.Equal("60d5ec49f1b2c8b4e8f1a1a3", dto.Id);
        Assert.Equal("Thai Garden", dto.RestaurantName);
        Assert.Equal("60d5ec49f1b2c8b4e8f1a1a4", dto.RestaurantId);
    }

    [Fact]
    public async Task GetReservation_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetReservationById("nonexistent")).Returns((Reservation?)null);

        // Act
        var result = await _controller.GetReservation("nonexistent");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task PostReservation_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var inputDto = new ReservationInputDto
        {
            RestaurantId = "60d5ec49f1b2c8b4e8f1a1a1",
            Date = DateTime.Parse("2024-07-30T20:00:00")
        };

        Reservation? capturedReservation = null;
        _mockService.Setup(s => s.AddReservation(It.IsAny<Reservation>()))
                    .Callback<Reservation>(r => capturedReservation = r);

        // Act
        var result = await _controller.PostReservation(inputDto);

        // Assert
        Assert.NotNull(result.Result);
        var createdResult = result.Result as CreatedAtActionResult;
        Assert.NotNull(createdResult);

        var dto = createdResult.Value as ReservationOutputDto;
        Assert.NotNull(dto);
        Assert.Equal(inputDto.Date, dto.Date);
        Assert.Equal(inputDto.RestaurantId, dto.RestaurantId);
    }

    [Fact]
    public async Task PutReservation_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetReservationById("5")).Returns((Reservation?)null);

        var input = new ReservationInputDto
        {
            Date = DateTime.Parse("2024-08-01T19:00:00")
        };

        // Act
        var result = await _controller.PutReservation("5", input);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task PutReservation_WhenExists_ShouldUpdateAndReturnOk()
    {
        // Arrange
        var existing = new Reservation
        {
            Id = "60d5ec49f1b2c8b4e8f1a1a5",
            RestaurantId = "60d5ec49f1b2c8b4e8f1a1a1",
            Date = DateTime.Parse("2024-07-22T19:00:00")
        };
        _mockService.Setup(s => s.GetReservationById("60d5ec49f1b2c8b4e8f1a1a5")).Returns(existing);

        var input = new ReservationInputDto
        {
            Date = DateTime.Parse("2024-07-23T20:00:00")
        };

        // Act
        var result = await _controller.PutReservation("60d5ec49f1b2c8b4e8f1a1a5", input);

        // Assert
        Assert.IsType<OkResult>(result);

        // Verify Edit was called
        _mockService.Verify(s => s.EditReservation(It.IsAny<Reservation>()), Times.Once);
    }

    [Fact]
    public async Task DeleteReservation_WhenNotFound_ShouldReturnNotFound()
    {
        // Arrange
        _mockService.Setup(s => s.GetReservationById("60d5ec49f1b2c8b4e8f1a1a5")).Returns((Reservation?)null);

        // Act
        var result = await _controller.DeleteReservation("60d5ec49f1b2c8b4e8f1a1a5");

        // Assert
        Assert.IsType<NotFoundResult>(result);

    }
    [Fact]
    public async Task DeleteReservation_WhenExists_ShouldDeleteAndReturnOk()
    {
        // Arrange
        var reservation = new Reservation
        {
            Id = "60d5ec49f1b2c8b4e8f1a1a5",
            RestaurantId = "60d5ec49f1b2c8b4e8f1a1a1",
            Date = DateTime.Parse("2024-07-22T19:00:00")
        };
        _mockService.Setup(s => s.GetReservationById("60d5ec49f1b2c8b4e8f1a1a5")).Returns(reservation);

        // Act
        var result = await _controller.DeleteReservation("60d5ec49f1b2c8b4e8f1a1a5");

        // Assert
        Assert.IsType<OkResult>(result);

        // Verify Delete was called
        _mockService.Verify(s => s.DeleteReservation(It.IsAny<Reservation>()), Times.Once);
    }
}
