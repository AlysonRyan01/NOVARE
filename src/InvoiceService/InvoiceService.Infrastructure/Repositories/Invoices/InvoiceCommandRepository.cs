using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Repositories.Invoices;
using InvoiceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedService.Shared;

namespace InvoiceService.Infrastructure.Repositories.Invoices;

public class InvoiceCommandRepository : IInvoiceCommandRepository
{
    private readonly ApplicationDataContext _context;

    public InvoiceCommandRepository(ApplicationDataContext context)
    {
        _context = context;
    }

    public async Task<Result<Invoice>> AddAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
        return Result<Invoice>.Ok(invoice);
    }

    public Task<Result<Invoice>> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _context.Invoices.Update(invoice);
        return Task.FromResult(Result<Invoice>.Ok(invoice));
    }

    public async Task<Result<Invoice>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

        if (invoice == null)
            return Result<Invoice>.Fail(["Invoice não encontrado"]);

        _context.Invoices.Remove(invoice);
        return Result<Invoice>.Ok(invoice);
    }
}