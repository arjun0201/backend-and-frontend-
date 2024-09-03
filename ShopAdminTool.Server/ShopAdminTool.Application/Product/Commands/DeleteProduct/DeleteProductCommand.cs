using MediatR;

namespace ShopAdminTool.Application;

public class DeleteProductCommand : IRequest
{
    public DeleteProductCommand(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

