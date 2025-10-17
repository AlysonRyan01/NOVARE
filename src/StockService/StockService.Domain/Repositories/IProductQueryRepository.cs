using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Domain.Repositories;

public interface IProductQueryRepository
{
    Task<Result<Product>> GetByIdAsync(Guid id, CancellationToken cancellationToken =  default);
    Task<Result<IEnumerable<Product>>> GetAllAsync(int pageNumber, int pageSize , CancellationToken cancellationToken =  default);
    Task<Result<bool>> ExistsAsync(Guid id, CancellationToken cancellationToken =  default);
}