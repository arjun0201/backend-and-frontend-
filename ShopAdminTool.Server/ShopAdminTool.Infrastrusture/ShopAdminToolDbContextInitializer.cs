using Microsoft.Extensions.DependencyInjection;
using ShopAdminTool.Core;
using Microsoft.Extensions.Hosting;

namespace ShopAdminTool.Infrastrusture;

public static class ShopAdminToolDbContextInitializer
{
    public static void ConfigureDb(this IServiceCollection services)
    {
        services.AddOptions<DatabaseSettings>()
            .BindConfiguration(nameof(DatabaseSettings))
            .ValidateOnStart();
        services.AddDbContext<ShopAdminToolDbContext>();
    }
    
    public static void SetupDb(this IHost host, bool isDevelopment)
    {
        using var scope = host.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        var context = services.GetService<ShopAdminToolDbContext>();

        if(context != null 
            && context.Database != null
            && context.Database.EnsureCreated()
            && isDevelopment)
        {
            Seed(context);
        }
    }

    public static void Seed(ShopAdminToolDbContext context)
    {
        context.Products.Add(new Product("testId1", "testName1", "testBrand1", 123, "testDescription1", 50));
        context.Products.Add(new Product("testId2", "testName2", "testBrand2", 123, "testDescription2", 0));
        context.Products.Add(new Product("testId3", "testName3", "testBrand1", 400, "testDescription3", 25));
        context.SaveChanges();
    }
}
