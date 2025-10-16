using Microsoft.EntityFrameworkCore;
using SharedService.Shared;
using StockService.Domain.Entities;
using StockService.Domain.Repositories;
using StockService.Infrastructure.Data.Contexts;

namespace ProductService.Infrastructure.Repositories;

public class ProductCommandRepository : IProductCommandRepository
{
    private readonly ApplicationDataContext  _context;

    public ProductCommandRepository(ApplicationDataContext context)
    {
        _context = context;
    }

    public async Task<Result<Product>> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        return Result<Product>.Ok(product);
    }

    public Task<Result<Product>> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        return Task.FromResult(Result<Product>.Ok(product));
    }

    public async Task<Result<Product>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _context.Products.FirstOrDefaultAsync(p => p.Id == id,  cancellationToken);
        if (result is null)
            return Result<Product>.Fail(["Produto não encontrado"]);
        
        _context.Products.Remove(result);
        return Result<Product>.Ok(result);
    }
}