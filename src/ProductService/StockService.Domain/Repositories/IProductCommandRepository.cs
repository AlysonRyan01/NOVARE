using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Domain.Repositories;

public interface IProductCommandRepository
{
    Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<Result<Product>> UpdateAsync(Product product,  CancellationToken cancellationToken = default);
    Task<Result<Product>> DeleteAsync(Guid id,  CancellationToken cancellationToken = default);
}