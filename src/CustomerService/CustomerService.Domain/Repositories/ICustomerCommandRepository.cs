using CustomerService.Domain.Entities;
using SharedService.Shared;

namespace CustomerService.Domain.Repositories;

public interface ICustomerCommandRepository
{
    Task<Result<Customer>> AddAsync(Customer customer, CancellationToken cancellationToken = default);
}