namespace ShopAdminTool.Core;

public interface IProductRepository
{
    public Task<IEnumerable<Product>> GetProducts();
    public Task<Product> GetProduct(string id);
    public Task CreateProduct(Product product);
    public Task UpdateProduct(Product product);
    public Task DeleteProduct(string id);
}
