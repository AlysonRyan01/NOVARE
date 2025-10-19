using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Enums;
using InvoiceService.Domain.Repositories.Invoices;
using InvoiceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedService.Shared;

namespace InvoiceService.Infrastructure.Repositories.Invoices;

public class InvoiceQueryRepository : IInvoiceQueryRepository
{
    private readonly ApplicationDataContext _context;

    public InvoiceQueryRepository(ApplicationDataContext context)
    {
        _context = context;
    }

    public async Task<Result<Invoice?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        return Result<Invoice?>.Ok(invoice);
    }

    public async Task<Result<Invoice?>> GetByNumberAsync(string number, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Number.Value == number, cancellationToken);

        return Result<Invoice?>.Ok(invoice);
    }

    public async Task<Result<List<Invoice>>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Items)
            .Where(i => i.CustomerId == customerId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<List<Invoice>>.Ok(invoices);
    }

    public async Task<Result<List<Invoice>>> GetByStatusAsync(EInvoiceStatus status, CancellationToken cancellationToken = default)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Items)
            .Where(i => i.Status == status)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

        return Result<List<Invoice>>.Ok(invoices);
    }

    public async Task<Result<IEnumerable<Invoice>>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            return Result<IEnumerable<Invoice>>.Fail(["Parâmetros de paginação inválidos"]);

        var skip = (pageNumber - 1) * pageSize;

        var invoices = await _context.Invoices
            .Include(i => i.Items)
            .OrderByDescending(i => i.CreatedAt)
            .Skip(skip)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Invoice>>.Ok(invoices);
    }
}