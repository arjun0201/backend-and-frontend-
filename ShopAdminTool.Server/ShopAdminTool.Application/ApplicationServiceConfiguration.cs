using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShopAdminTool.Core;
using ShopAdminTool.Infrastrusture;

namespace ShopAdminTool.Application;

public static class ApplicationServiceConfiguration
{
    public static void ConfigureApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.ConfigureDb();

        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile<ProductMappingProfile>();
        });
        var mapper = mappingConfig.CreateMapper();
        services.AddSingleton(mapper);
    }
    
    public static void ConfigureApplication(this IHost app, bool isDevelopment)
    {
        app.SetupDb(isDevelopment);
    }
}

