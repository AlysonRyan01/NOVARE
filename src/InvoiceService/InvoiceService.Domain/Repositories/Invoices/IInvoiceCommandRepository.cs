using InvoiceService.Domain.AggregateRoots;
using SharedService.Shared;

namespace InvoiceService.Domain.Repositories.Invoices;

public interface IInvoiceCommandRepository
{
    Task<Result<Invoice>> AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Result<Invoice>> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Result<Invoice>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}