using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ShopAdminTool.Application;

public class ProductDto
{
    [JsonConstructor]
    public ProductDto(string id, string name, string brand, int price, string description, int stock)
    {
        Id = id;
        Name = name;
        Brand = brand;
        Price = price;
        Description = description;
        Stock = stock;
        
    }

    [Key]
    [Required]
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [Required]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [Required]
    [JsonPropertyName("brand")]
    public string Brand { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    [JsonPropertyName("price")]
    public int Price { get; set; }
    
    [Required]
    [JsonPropertyName("description")]
    public string Description { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    [JsonPropertyName("stock")]
    public int Stock { get; set; }
}