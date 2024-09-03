using MediatR;

namespace ShopAdminTool.Application;

public class CreateProductCommand : IRequest
{
    public CreateProductCommand(ProductDto product)
    {
        Product = product;
    }

    public ProductDto Product { get; }
}

