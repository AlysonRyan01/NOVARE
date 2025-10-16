using SharedService.Shared;
using StockService.Domain.Abstractions;
using StockService.Domain.ValueObjects.Product;

namespace StockService.Domain.Entities;

public class Product : Entity
{
    public Name Name { get; internal set; } = null!;
    public Description Description { get; internal set; } = null!;
    public StockQuantity StockQuantity { get; internal set; } = null!;
    public Price Price { get; internal set; } = null!;
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    internal Product() { }
    
    public bool HasSufficientStock(int requestedQuantity)
        => StockQuantity.Value >= requestedQuantity;
    
    public Result<Product> DecreaseStock(int quantity)
    {
        var result = StockQuantity.Subtract(quantity);
        if (!result.IsSuccess)
            return Result<Product>.Fail(result.Errors!);
        
        StockQuantity = result.Value!;
        
        UpdatedAt = DateTime.UtcNow;

        return Result<Product>.Ok(this);
    }
    
    public Result<Product> IncreaseStock(int quantity)
    {
        var result = StockQuantity.Add(quantity);
        if (!result.IsSuccess)
            return Result<Product>.Fail(result.Errors!);
        
        StockQuantity = result.Value!;
        
        UpdatedAt = DateTime.UtcNow;

        return Result<Product>.Ok(this);
    }
    
    public Result<Product> UpdateBasicInfo(Name name, Description description, Price price)
    {
        Name = name;
        Description = description;
        Price = price;
        UpdatedAt = DateTime.UtcNow;

        return Result<Product>.Ok(this);
    }
}