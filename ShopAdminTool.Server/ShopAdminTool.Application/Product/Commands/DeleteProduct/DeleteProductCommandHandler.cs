using MediatR;
using ShopAdminTool.Core;

namespace ShopAdminTool.Application;

public class DeleteProductCommandHandler: IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await _productRepository.DeleteProduct(request.Id);
    }
}
