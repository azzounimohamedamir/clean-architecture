using Application.Common.Interfaces;
using Application.Products.Commands.CreateProduct;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Products.Commands;

public class CreateProductCommandTests
{
    private readonly Mock<IApplicationDbContext> _context;

    public CreateProductCommandTests()
    {
        _context = new Mock<IApplicationDbContext>();
    }

    [Fact]
    public async Task Handle_ShouldCreateProduct_WhenValidCommand()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m
        };

        var handler = new CreateProductCommandHandler(_context.Object);
        var products = new List<Product>();

        _context.Setup(x => x.Products.Add(It.IsAny<Product>()))
            .Callback<Product>(products.Add);

        _context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeGreaterThan(0);
        products.Should().HaveCount(1);
        products[0].Name.Should().Be(command.Name);
        products[0].Description.Should().Be(command.Description);
        products[0].Price.Should().Be(command.Price);
        _context.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetCreatedAt_WhenCreatingProduct()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m
        };

        var handler = new CreateProductCommandHandler(_context.Object);
        var products = new List<Product>();

        _context.Setup(x => x.Products.Add(It.IsAny<Product>()))
            .Callback<Product>(products.Add);

        _context.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        products[0].CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}
