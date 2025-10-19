using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using CustomerService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedService.Shared;

namespace CustomerService.Infrastructure.Repositories;

public class CustomerQueryRepository : ICustomerQueryRepository
{
    private readonly ApplicationDataContext _context;

    public CustomerQueryRepository(ApplicationDataContext context)
    {
        _context = context;
    }

    public async Task<Result<Customer>> GetAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

        if (customer == null)
            return Result<Customer>.Fail([$"Cliente com Id {customerId} não encontrado."]);

        return Result<Customer>.Ok(customer);
    }

    public async Task<Result<List<Customer>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result<List<Customer>>.Ok(customers);
    }
}