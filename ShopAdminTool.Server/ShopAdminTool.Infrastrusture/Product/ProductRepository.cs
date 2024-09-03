using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopAdminTool.Core;
using ShopAdminTool.Core.Exceptions;
using ShopAdminTool.Infrastrusture.Resources;

namespace ShopAdminTool.Infrastrusture;

public class ProductRepository : IProductRepository
{
    private readonly ShopAdminToolDbContext _context;
    private readonly ILogger<ProductRepository> _logger;

    public ProductRepository(ShopAdminToolDbContext context, ILogger<ProductRepository> logger)
    {
        _logger = logger;
        _context = context;
    }

    public async Task CreateProduct(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id);
        if (existingProduct != null)
        {
            throw new AleadyExistsException(string.Format(ErrorMessages.ProductAlreadyExistsErrorMessage, product.Id));
        }

        await _context.Products.AddAsync(product);
        _logger.LogInformation(string.Format(GeneralMessages.ProductCreatedMessage, product.Id));
        await _context.SaveChangesAsync();
    }

    public async Task DeleteProduct(string id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product != null)
        {
            _context.Products.Remove(product);
            _logger.LogInformation(string.Format(GeneralMessages.ProductDeletedMessage, id));
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new NotFoundException(string.Format(ErrorMessages.ProductNotFoundErrorMessage, id));
        }
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _context.Products.ToArrayAsync();
    }

    public async Task<Product> GetProduct(string id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            throw new NotFoundException(string.Format(ErrorMessages.ProductNotFoundErrorMessage, id));
        }
        return product;
    }

    public async Task UpdateProduct(Product product)
    {
        var existingProduct = await _context.Products.FindAsync(product.Id);
        if (existingProduct == null)
        {
            throw new NotFoundException(string.Format(ErrorMessages.ProductNotFoundErrorMessage, product.Id));
        }

        _context.Products.Entry(existingProduct).CurrentValues.SetValues(product);
        _logger.LogInformation(string.Format(GeneralMessages.ProductUpdatedMessage, product.Id));
        await _context.SaveChangesAsync();
    }

}
