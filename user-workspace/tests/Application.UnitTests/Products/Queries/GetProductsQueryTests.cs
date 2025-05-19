using Application.Common.Interfaces;
using Application.Products.DTOs;
using Application.Products.Queries.GetProducts;
using AutoMapper;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests.Products.Queries;

public class GetProductsQueryTests
{
    private readonly Mock<IApplicationDbContext> _context;
    private readonly Mock<IMapper> _mapper;

    public GetProductsQueryTests()
    {
        _context = new Mock<IApplicationDbContext>();
        _mapper = new Mock<IMapper>();
    }

    [Fact]
    public async Task Handle_ShouldReturnMappedProducts_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Product 1", Price = 10.99m, CreatedAt = DateTime.UtcNow },
            new() { Id = 2, Name = "Product 2", Price = 20.99m, CreatedAt = DateTime.UtcNow }
        };

        var productDtos = new List<ProductDto>
        {
            new() { Id = 1, Name = "Product 1", Price = 10.99m },
            new() { Id = 2, Name = "Product 2", Price = 20.99m }
        };

        var mockDbSet = products.AsQueryable().BuildMockDbSet();
        _context.Setup(x => x.Products).Returns(mockDbSet.Object);
        
        _mapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg => 
                cfg.CreateMap<Product, ProductDto>()));

        var handler = new GetProductsQueryHandler(_context.Object, _mapper.Object);
        var query = new GetProductsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _context.Verify(x => x.Products, Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoProductsExist()
    {
        // Arrange
        var products = new List<Product>();
        var mockDbSet = products.AsQueryable().BuildMockDbSet();
        _context.Setup(x => x.Products).Returns(mockDbSet.Object);

        _mapper.Setup(m => m.ConfigurationProvider)
            .Returns(new MapperConfiguration(cfg => 
                cfg.CreateMap<Product, ProductDto>()));

        var handler = new GetProductsQueryHandler(_context.Object, _mapper.Object);
        var query = new GetProductsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}

public static class MockDbSetExtensions
{
    public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
    {
        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
        return mockSet;
    }
}
