using AutoFixture.Xunit2;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using ShopAdminTool.Core;
using ShopAdminTool.Core.Exceptions;
using ShopAdminTool.Infrastrusture;
using ShopAdminTool.Infrastrusture.Resources;

namespace ShopAdminTool.Infrastructure.Tests;

public class ProductRepositoryTests
{
    private readonly Mock<ShopAdminToolDbContext> _context;
    private readonly Mock<ILogger<ProductRepository>> _logger;
    private readonly IProductRepository _repository;

    public ProductRepositoryTests()
    {
        var databaseOptions = new Mock<IOptions<DatabaseSettings>>();
        _context = new Mock<ShopAdminToolDbContext>(new DbContextOptions<ShopAdminToolDbContext>(), databaseOptions.Object);
        _context
            .Setup(x => x.Products)
            .ReturnsDbSet(new List<Product>());

        _logger = new Mock<ILogger<ProductRepository>>();
        _repository = new ProductRepository(_context.Object, _logger.Object);
    }

    [Theory, AutoData]
    public async void CreateProduct_NotExistingProduct_ShouldCreateAndLog(Product product)
    {
        await _repository.CreateProduct(product);

        _context.Verify(c => c.Products.FindAsync(product.Id), Times.Once);
        _context.Verify(c => c.Products.AddAsync(product, It.IsAny<CancellationToken>()), Times.Once);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        CheckLog(string.Format(GeneralMessages.ProductCreatedMessage, product.Id), Times.Once);
        
    }

    [Theory, AutoData]
    public async void CreateProduct_ExistingProduct_ShouldThrowException(Product product)
    {
        _context.Setup(c => c.Products.FindAsync(It.IsAny<string>())).ReturnsAsync(product);
        try
        {
            await _repository.CreateProduct(product);
            Assert.Fail("Exception expected");
        }
        catch(Exception ex)
        {
            Assert.True(ex is AleadyExistsException);
            Assert.Equivalent(ex.Message, string.Format(ErrorMessages.ProductAlreadyExistsErrorMessage, product.Id));
        }

        _context.Verify(c => c.Products.FindAsync(product.Id), Times.Once);
        _context.Verify(c => c.Products.AddAsync(product, It.IsAny<CancellationToken>()), Times.Never);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        CheckLog(string.Format(GeneralMessages.ProductCreatedMessage, product.Id), Times.Never);
    }

    [Theory, AutoData]
    public async void DeleteProduct_ExistingProduct_ShouldDeleteAndLog(Product product)
    {
        _context.Setup(c => c.Products.FindAsync(It.IsAny<string>())).ReturnsAsync(product);

        await _repository.DeleteProduct(product.Id);

        _context.Verify(c => c.Products.FindAsync(product.Id), Times.Once);
        _context.Verify(c => c.Products.Remove(product), Times.Once);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        CheckLog(string.Format(GeneralMessages.ProductDeletedMessage, product.Id), Times.Once);
        
    }

    [Theory, AutoData]
    public async void DeleteProduct_NotExistingProduct_ShouldThrowException(Product product)
    {
        try
        {
            await _repository.DeleteProduct(product.Id);
            Assert.Fail("Exception expected");
        }
        catch(Exception ex)
        {
            Assert.True(ex is NotFoundException);
            Assert.Equivalent(ex.Message, string.Format(ErrorMessages.ProductNotFoundErrorMessage, product.Id));
        }

        _context.Verify(c => c.Products.FindAsync(product.Id), Times.Once);
        _context.Verify(c => c.Products.Remove(product), Times.Never);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        CheckLog(string.Format(GeneralMessages.ProductDeletedMessage, product.Id), Times.Never);
    }

    [Theory, AutoData]
    public async void UpdateProduct_ExistingProduct_ShouldUpdateAndLog(Product product)
    {
        var entry = new Mock<EntityEntry<Product>>(It.IsAny<It.IsAnyType>());
        entry.Setup(c => c.CurrentValues).Returns(new Mock<PropertyValues>(It.IsAny<It.IsAnyType>()).Object);
        _context.Setup(c => c.Products.Entry(It.IsAny<Product>())).Returns(entry.Object);

        _context.Setup(c => c.Products.FindAsync(It.IsAny<string>())).ReturnsAsync(product);

        await _repository.UpdateProduct(product);

        _context.Verify(c => c.Products.FindAsync(product.Id), Times.Once);
        _context.Verify(c => c.Products.Entry(product), Times.Once);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        CheckLog(string.Format(GeneralMessages.ProductUpdatedMessage, product.Id), Times.Once);
    }

    [Theory, AutoData]
    public async void UpdateProduct_NotExistingProduct_ShouldThrowException(Product product)
    {
        try
        {
            await _repository.UpdateProduct(product);
            Assert.Fail("Exception expected");
        }
        catch(Exception ex)
        {
            Assert.True(ex is NotFoundException);
            Assert.Equivalent(ex.Message, string.Format(ErrorMessages.ProductNotFoundErrorMessage, product.Id));
        }

        _context.Verify(c => c.Products.FindAsync(product.Id), Times.Once);
        _context.Verify(c => c.Products.Entry(product), Times.Never);
        _context.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        CheckLog(string.Format(GeneralMessages.ProductUpdatedMessage, product.Id), Times.Never);
    }

    [Theory, AutoData]
    public async void GetProduct_ExistingProduct_ShouldGetProduct(Product expectedProduct)
    {
        _context.Setup(c => c.Products.FindAsync(It.IsAny<string>())).ReturnsAsync(expectedProduct);

        var product = await _repository.GetProduct(expectedProduct.Id);

        _context.Verify(c => c.Products.FindAsync(expectedProduct.Id), Times.Once);
        Assert.Equivalent(product, expectedProduct);
    }

    [Theory, AutoData]
    public async void GetProduct_NotExistingProduct_ShouldThrowException(Product product)
    {
        try
        {
            await _repository.GetProduct(product.Id);
            Assert.Fail("Exception expected");
        }
        catch(Exception ex)
        {
            Assert.True(ex is NotFoundException);
            Assert.Equivalent(ex.Message, string.Format(ErrorMessages.ProductNotFoundErrorMessage, product.Id));
        }

        _context.Verify(c => c.Products.FindAsync(product.Id), Times.Once);
    }

    [Theory, AutoData]
    public async void GetProducts_ExistingProducts_ShouldGetProducts(List<Product> expectedProducts)
    {
        _context
            .Setup(x => x.Products)
            .ReturnsDbSet(expectedProducts);

        var products = await _repository.GetProducts();

        Assert.Equivalent(products, expectedProducts);
    }

    private void CheckLog(string message, Func<Times> times)
    {
        _logger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == message && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            times);
    }
}