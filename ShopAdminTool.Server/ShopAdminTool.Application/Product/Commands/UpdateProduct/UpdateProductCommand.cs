using MediatR;

namespace ShopAdminTool.Application;

public class UpdateProductCommand : IRequest
{
    public UpdateProductCommand(string id, ProductDto product)
    {
        Id = id;
        Product = product;
    }

    public string Id { get; }
    public ProductDto Product { get; }
}

