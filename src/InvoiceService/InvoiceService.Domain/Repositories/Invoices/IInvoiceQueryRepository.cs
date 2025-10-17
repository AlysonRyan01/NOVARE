using InvoiceService.Domain.AggregateRoots;
using SharedService.Shared;

namespace InvoiceService.Domain.Repositories.Invoices;

public interface IInvoiceQueryRepository
{
    Task<Result<Invoice?>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<Invoice>>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}