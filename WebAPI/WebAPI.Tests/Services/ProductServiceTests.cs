using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace WebAPI.Tests.Services;

public class ProductServiceTests : IDisposable
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid():N}";
    private ApplicationDbContext? _context;

    public ProductServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
        _context = new ApplicationDbContext(options);
    }

    private void ReloadContext()
    {
        _context!.Dispose();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
        _context = new ApplicationDbContext(options);
    }

    public void Dispose()
    {
        _context?.Database.EnsureDeleted();
        _context?.Dispose();
    }

    [Fact]
    public async Task Add_Product_ShouldInsertIntoDatabase()
    {
        // Arrange
        var service = new ProductService(_context!);
        var product = new Product { Name = "Test Product", Price = 9.99m };

        // Act
        var added = await service.AddAsync(product);
        
        // Assert
        Assert.NotNull(added);
        Assert.Equal(product.Name, added!.Name);
        Assert.Equal(product.Price, added.Price);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllProducts()
    {
        // Arrange
        await _context!.Products.AddRangeAsync(
            new Product { Name = "Product A", Price = 10m },
            new Product { Name = "Product B", Price = 20m }
        );
        await _context.SaveChangesAsync();

        var service = new ProductService(_context!);

        // Act
        PagedResult<Product> result = await service.GetAllAsync(0, 10);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(10, result.PageSize);
        Assert.Equal(1, result.CurrentPage);
    }

    [Fact]
    public async Task GetById_ShouldReturnProduct_WhenExists()
    {
        // Arrange
        var expected = new Product { Name = "Found Product", Price = 15m };
        _context!.Products.Add(expected);
        _context.SaveChanges();
        ReloadContext();

        var service = new ProductService(_context!);

        // Act
        var result = await service.GetByIdAsync(expected.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Found Product", result!.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenDoesNotExist()
    {
        // Arrange
        ReloadContext();
        var service = new ProductService(_context!);

        // Act
        var result = await service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Update_Product_ShouldModifyExistingProduct()
    {
        // Arrange
        var product = new Product { Name = "Old Name", Price = 5m };
        _context!.Products.Add(product);
        _context.SaveChanges();
        ReloadContext();

        var service = new ProductService(_context!);
        
        // Act
        product.Name = "New Name";
        product.Price = 10m;
        var updated = await service.UpdateAsync(product);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("New Name", updated!.Name);
        Assert.Equal(10m, updated.Price);
    }

    [Fact]
    public async Task Delete_Product_ShouldRemoveFromDatabase()
    {
        // Arrange
        var product = new Product { Name = "To Delete", Price = 7.5m };
        _context!.Products.Add(product);
        _context.SaveChanges();
        ReloadContext();

        var service = new ProductService(_context!);

        // Act
        await service.DeleteAsync(product.Id);

        // Assert
        var deleted = _context!.Products.Find(product.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Delete_NonExistentProduct_ShouldNotThrow()
    {
        // Arrange
        ReloadContext();
        var service = new ProductService(_context!);

        // Act & Assert - should not throw
        var ex = await Record.ExceptionAsync(async () => await service.DeleteAsync(999));
        Assert.Null(ex);
    }
}