using CustomerService.Domain.Entities;
using SharedService.Shared;

namespace CustomerService.Domain.Repositories;

public interface ICustomerQueryRepository
{
    Task<Result<Customer>> GetAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Result<List<Customer>>> GetAllAsync(CancellationToken cancellationToken = default);
}