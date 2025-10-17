using InvoiceService.Domain.Entities;
using InvoiceService.Domain.Repositories.Customers;
using InvoiceService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SharedService.Shared;

namespace InvoiceService.Infrastructure.Repositories.Customers;

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
            .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

        if (customer == null)
            return Result<Customer>.Fail(["Cliente não encontrado"]);

        return Result<Customer>.Ok(customer);
    }

    public async Task<Result<List<Customer>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _context.Customers
            .OrderBy(c => c.Name.Value)
            .ToListAsync(cancellationToken);

        return Result<List<Customer>>.Ok(customers);
    }
}