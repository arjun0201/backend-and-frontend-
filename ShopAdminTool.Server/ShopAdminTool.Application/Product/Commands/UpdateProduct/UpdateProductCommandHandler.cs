using AutoMapper;
using MediatR;
using ShopAdminTool.Core;

namespace ShopAdminTool.Application;

public class UpdateProductCommandHandler: IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        await _productRepository.UpdateProduct(_mapper.Map<Product>(request.Product));
    }
}
