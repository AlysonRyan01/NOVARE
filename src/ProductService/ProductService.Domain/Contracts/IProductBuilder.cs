using ProductService.Domain.Entities;
using ProductService.Domain.ValueObjects.Product;
using SharedService.Shared;

namespace ProductService.Domain.Contracts;

public interface IProductBuilder
{
    IProductBuilder WithName(Name name);
    IProductBuilder WithDescription(Description description);
    IProductBuilder WithPrice(Price price);
    IProductBuilder WithStockQuantity(StockQuantity stockQuantity);
    Result<Product> Build();
}