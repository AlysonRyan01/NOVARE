using InvoiceService.Domain.Entities;
using InvoiceService.Domain.Repositories.Customers;
using InvoiceService.Infrastructure.Data;
using SharedService.Shared;

namespace InvoiceService.Infrastructure.Repositories.Customers;

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