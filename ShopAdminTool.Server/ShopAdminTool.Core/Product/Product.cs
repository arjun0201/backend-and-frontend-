using System.ComponentModel.DataAnnotations;

namespace ShopAdminTool.Core;

public class Product
{
    public Product(string id, string name, string brand, int price, string description, int stock)
    {
        Id = id;
        Name = name;
        Brand = brand;
        Price = price;
        Description = description;
        Stock = stock;
        
    }

    [Key]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Brand { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Price { get; set; }
    public string Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
    public int Stock { get; set; }
}
