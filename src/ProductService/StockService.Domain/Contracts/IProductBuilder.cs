using StockService.Domain.Entities;
using StockService.Domain.ValueObjects.Product;
using SharedService.Shared;

namespace StockService.Domain.Contracts;

public interface IProductBuilder
{
    IProductBuilder WithName(Name name);
    IProductBuilder WithDescription(Description description);
    IProductBuilder WithPrice(Price price);
    IProductBuilder WithStockQuantity(StockQuantity stockQuantity);
    Result<Product> Build();
}