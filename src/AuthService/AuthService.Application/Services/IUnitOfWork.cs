namespace AuthService.Application.Services;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();
    Task<int> CommitAsync();
    Task RollbackAsync();
}