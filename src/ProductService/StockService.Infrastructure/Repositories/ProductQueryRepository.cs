using Microsoft.EntityFrameworkCore;
using SharedService.Shared;
using StockService.Domain.Entities;
using StockService.Domain.Repositories;
using StockService.Infrastructure.Data.Contexts;

namespace ProductService.Infrastructure.Repositories;

public class ProductQueryRepository : IProductQueryRepository
{
    private readonly ApplicationDataContext _context;

    public ProductQueryRepository(ApplicationDataContext context)
    {
        _context = context;
    }

    public async Task<Result<Product>> GetByIdAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            return Result<Product>.Fail(["O ID do produto é necessário"]);
        
        var result = await WithIncludes(_context.Products)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if  (result == null)
            return Result<Product>.Fail(["Produto não encontrado"]);
        
        return Result<Product>.Ok(result);
    }

    public async Task<Result<IEnumerable<Product>>> GetAllAsync(
        int pageNumber, 
        int pageSize, 
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
            return Result<IEnumerable<Product>>.Fail(["O número da página deve ser maior que 0"]);

        if (pageSize < 1 || pageSize > 100)
            return Result<IEnumerable<Product>>.Fail(["O tamanho da página deve estar entre 1 e 100"]);
        
        var skip = (pageNumber - 1) * pageSize;
        
        var products = await WithIncludes(_context.Products)
            .OrderBy(p => p.Name.Value)
            .Skip(skip)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result<IEnumerable<Product>>.Ok(products);
    }

    public async Task<Result<bool>> ExistsAsync(
        Guid id, 
        CancellationToken cancellationToken = default)
    {
        var exists = await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
        return Result<bool>.Ok(exists);
    }

    private IQueryable<Product> WithIncludes(IQueryable<Product> query)
    {
        return query
            .Include(p => p.Name)
            .Include(p => p.Description)
            .Include(p => p.Price)
            .Include(p => p.StockQuantity);
    }
}