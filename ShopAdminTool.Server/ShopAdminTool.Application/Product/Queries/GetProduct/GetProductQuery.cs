using MediatR;

namespace ShopAdminTool.Application;

public class GetProductQuery : IRequest<ProductDto>
{
    public GetProductQuery(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

