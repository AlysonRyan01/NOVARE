using InvoiceService.Domain.Entities;
using SharedService.Shared;

namespace InvoiceService.Domain.Repositories.Customers;

public interface ICustomerQueryRepository
{
    Task<Result<Customer>> GetAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<List<Customer>>> GetAllAsync(CancellationToken cancellationToken = default);
}