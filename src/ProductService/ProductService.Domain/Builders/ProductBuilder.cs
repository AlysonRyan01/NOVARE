using ProductService.Domain.Contracts;
using ProductService.Domain.Entities;
using ProductService.Domain.ValueObjects.Product;
using SharedService.Shared;

namespace ProductService.Domain.Builders;

public class ProductBuilder : IProductBuilder
{
    private Name? _name;
    private Description? _description;
    private Price? _price;
    private StockQuantity? _stockQuantity;
    
    public IProductBuilder WithName(Name name)
    {
        _name = name;
        return this;
    }

    public IProductBuilder WithDescription(Description description)
    {
        _description = description;
        return this;
    }

    public IProductBuilder WithPrice(Price price)
    {
        _price = price;
        return this;
    }

    public IProductBuilder WithStockQuantity(StockQuantity stockQuantity)
    {
        _stockQuantity = stockQuantity;
        return this;
    }

    public Result<Product> Build()
    {
        if (_name == null)
            return Result<Product>.Fail(["O nome do produto é obrigatório"]);
        
        if (_description == null)
            return Result<Product>.Fail(["A descrição é obrigatória"]);
            
        if (_price == null)
            return Result<Product>.Fail(["O preço é obrigatório"]);
            
        if (_stockQuantity == null)
            return Result<Product>.Fail(["O estoque é obrigatório"]);
        
        var product = new Product
        {
            Name = _name,
            Description = _description,
            Price = _price,
            StockQuantity = _stockQuantity
        };
        
        return Result<Product>.Ok(product);
    }
}