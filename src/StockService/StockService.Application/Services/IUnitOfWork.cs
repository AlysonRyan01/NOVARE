namespace StockService.Application.Services;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
}