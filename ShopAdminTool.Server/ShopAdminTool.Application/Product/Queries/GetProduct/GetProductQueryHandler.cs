using AutoMapper;
using MediatR;
using ShopAdminTool.Core;

namespace ShopAdminTool.Application;

public class GetProductQueryHandler: IRequestHandler<GetProductQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        return _mapper.Map<ProductDto>(await _productRepository.GetProduct(request.Id));
    }
}
