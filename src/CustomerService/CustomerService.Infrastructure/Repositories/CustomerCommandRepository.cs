using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using CustomerService.Infrastructure.Data;
using SharedService.Shared;

namespace CustomerService.Infrastructure.Repositories;

public class CustomerCommandRepository : ICustomerCommandRepository
{
    private readonly ApplicationDataContext _context;

    public CustomerCommandRepository(ApplicationDataContext context)
    {
        _context = context;
    }
    
    public async Task<Result<Customer>> AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await _context.Customers.AddAsync(customer, cancellationToken);
        return Result<Customer>.Ok(customer);
    }
}