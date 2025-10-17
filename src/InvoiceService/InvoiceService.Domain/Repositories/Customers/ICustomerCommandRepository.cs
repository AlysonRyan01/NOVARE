using InvoiceService.Domain.Entities;
using SharedService.Shared;

namespace InvoiceService.Domain.Repositories.Customers;

public interface ICustomerCommandRepository
{
    Task<Result<Customer>> AddAsync(Customer customer, CancellationToken cancellationToken = default);
}