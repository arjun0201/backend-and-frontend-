using MediatR;
using ShopAdminTool.Core;
using AutoMapper;

namespace ShopAdminTool.Application;

public class GetProductsQueryHandler: IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return _mapper.Map<IEnumerable<ProductDto>>(await _productRepository.GetProducts());
    }

}
