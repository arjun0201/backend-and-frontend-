
using System.Net;
using System.Net.Http.Json;
using System.Text;
using AutoFixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ShopAdminTool.Api.Middleware;
using ShopAdminTool.Api.Resources;
using ShopAdminTool.Application;

namespace ShopAdminTool.Api.IntegrationTests;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private string _apiKey;

    private const string _apiUrl = "/api/products";
    private ProductDto _testProduct;
    private HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        _apiKey = config[ApiKeyMiddleware.ApiKeyName] ?? "";

        var fixture = new Fixture();
        _testProduct = fixture.Create<ProductDto>();

        _client = _factory.CreateClient();
        _client.DefaultRequestHeaders.Add(GenericMessages.ApiKey, _apiKey);
    }

    public async void Dispose()
    {
        await DeleteProduct(_testProduct.Id);
        _client.Dispose();
    }

    [Fact]
    public async void GetProducts_WithoutApiKey_ShouldReturnUnauthorized()
    {
        using var client = _factory.CreateClient();

        HttpResponseMessage response = await client.GetAsync(_apiUrl);
        
        Assert.Equivalent(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async void CreateProduct_NotExistingProduct_ShouldReturnCreated()
    {
        var response = await CreateProduct(_testProduct);
        
        response.EnsureSuccessStatusCode();

        Assert.Equivalent(HttpStatusCode.Created, response.StatusCode);

        await DeleteProduct(_testProduct.Id);
    }

    [Fact]
    public async void CreateProduct_ExistintProduct_ShouldReturnBadRequest()
    {
        await CreateProduct(_testProduct);
        
        var response = await CreateProduct(_testProduct);
        
        Assert.Equivalent(HttpStatusCode.BadRequest, response.StatusCode);

        await DeleteProduct(_testProduct.Id);
    }

    [Fact]
    public async void DeleteProduct_ExistingProduct_ShouldReturnOk()
    {
        await CreateProduct(_testProduct);

        var response = await DeleteProduct(_testProduct.Id);
        
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async void DeleteProduct_NotExistingProduct_ShouldReturnNotFound()
    {
        var response = await DeleteProduct(_testProduct.Id);
        
        Assert.Equivalent(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async void UpdateProduct_ExistingProduct_ShouldReturnOk()
    {
        await CreateProduct(_testProduct);

        var updatedProduct = new ProductDto(_testProduct.Id, _testProduct.Name, "New Value", _testProduct.Price, _testProduct.Description, _testProduct.Stock);
        var updateResponse = await UpdateProduct(updatedProduct);
        
        updateResponse.EnsureSuccessStatusCode();

        var getResponse = await GetByIdProduct(updatedProduct.Id);

        var product = await getResponse.Content.ReadFromJsonAsync<ProductDto>();

        Assert.Equivalent(updatedProduct, product);

        await DeleteProduct(_testProduct.Id);
    }

    [Fact]
    public async void UpdateProduct_NotExistingProduct_ShouldReturnNotFound()
    {
        var updatedProduct = new ProductDto(_testProduct.Id, _testProduct.Name, "New Value", _testProduct.Price, _testProduct.Description, _testProduct.Stock);
        var updateResponse = await UpdateProduct(updatedProduct);
        
        Assert.Equivalent(HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact]
    public async void GetProduct_ExistingProduct_ShouldReturnOk()
    {
        await CreateProduct(_testProduct);
        var response = await GetByIdProduct(_testProduct.Id);

        response.EnsureSuccessStatusCode();
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();

        Assert.Equivalent(_testProduct, product);

        await DeleteProduct(_testProduct.Id);
    }

    [Fact]
    public async void GetProduct_NotExistingProduct_ShouldReturnNotFound()
    {
        var response = await GetByIdProduct(_testProduct.Id);
        
        Assert.Equivalent(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async void GetProducts_ShouldReturnProducts()
    {
        await CreateProduct(_testProduct);

        HttpResponseMessage response = await GetProducts();

        var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        
        response.EnsureSuccessStatusCode();

        Assert.NotNull(products);
        Assert.NotEmpty(products);
        Assert.True(products.Count() > 0);

        await DeleteProduct(_testProduct.Id);
    }

    public async Task<HttpResponseMessage> DeleteProduct(string productId)
    {
        return await _client.DeleteAsync($"{_apiUrl}/{productId}");
    }

    public async Task<HttpResponseMessage> CreateProduct(ProductDto product)
    {
        var stringPayload = JsonConvert.SerializeObject(product);
        var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

        return await _client.PostAsync(_apiUrl, httpContent);
    }

    public async Task<HttpResponseMessage> UpdateProduct(ProductDto product)
    {
        var stringPayload = JsonConvert.SerializeObject(product);
        var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

        return await _client.PatchAsync($"{_apiUrl}/{product.Id}", httpContent);
    }

    public async Task<HttpResponseMessage> GetByIdProduct(string productId)
    {
        return await _client.GetAsync($"{_apiUrl}/{productId}");
    }

    public async Task<HttpResponseMessage> GetProducts()
    {
        return await _client.GetAsync(_apiUrl);
    }
}