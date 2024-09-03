using AutoMapper;
using ShopAdminTool.Core;

namespace ShopAdminTool.Application;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
    }
}
