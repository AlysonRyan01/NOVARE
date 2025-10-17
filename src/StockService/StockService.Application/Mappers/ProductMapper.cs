using SharedService.Shared.Dtos;

namespace ProductService.Application.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(this StockService.Domain.Entities.Product product)
    {
        return new ProductDto(
            product.Id,
            product.Name.Value,
            product.Description.Value,
            product.Price.Value,
            product.StockQuantity.Value,
            product.CreatedAt,
            product.UpdatedAt
        );
    }

    public static List<ProductDto> ToDto(this IEnumerable<StockService.Domain.Entities.Product> products)
    {
        return products.Select(p => p.ToDto()).ToList();
    }
}