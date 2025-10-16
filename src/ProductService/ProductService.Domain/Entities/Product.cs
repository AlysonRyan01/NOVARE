using ProductService.Domain.Abstractions;
using ProductService.Domain.ValueObjects.Product;

namespace ProductService.Domain.Entities;

public class Product : Entity
{
    public Name Name { get; internal set; } = null!;
    public Description Description { get; internal set; } = null!;
    public StockQuantity StockQuantity { get; internal set; } = null!;
    public Price Price { get; internal set; } = null!;

    internal Product() { }
    
    
}