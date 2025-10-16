using AuthService.Application.Services;
using AuthService.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AuthService.Infrastructure.Services;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly ApplicationDataContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(ApplicationDataContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress");
        }

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task<int> CommitAsync()
    {
        try
        {
            int result = await _context.SaveChangesAsync();

            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }

            return result;
        }
        catch
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _transaction = null;
            _disposed = true;
        }
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (!_disposed)
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }

            await _context.DisposeAsync();
            _transaction = null;
            _disposed = true;
        }
    }
}