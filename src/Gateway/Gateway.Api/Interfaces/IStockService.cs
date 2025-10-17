using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Interfaces;

public interface IStockService
{
    Task<Result<IEnumerable<ProductDto>>> GetAllProducts(int pageNumber, int pageSize);
    Task<Result<ProductDto>> GetProductById(Guid productId);
    Task<Result<ProductDto>> CreateProduct(CreateProductDto productDto);
    Task<Result<ProductDto>> UpdateProduct(UpdateProductDto productDto);
    Task<Result<Guid>> DeleteProduct(Guid productId);
    Task<Result<ProductDto>> IncreaseStock(IncreaseStockDto increaseStockDto);
    Task<Result<ProductDto>> DecreaseStock(DecreaseStockDto increaseStockDto);
}