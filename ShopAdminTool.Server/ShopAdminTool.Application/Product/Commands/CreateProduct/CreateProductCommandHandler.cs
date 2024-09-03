using AutoMapper;
using MediatR;
using ShopAdminTool.Core;

namespace ShopAdminTool.Application;

public class CreateProductCommandHandler: IRequestHandler<CreateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        await _productRepository.CreateProduct(_mapper.Map<Product>(request.Product));
    }
}
