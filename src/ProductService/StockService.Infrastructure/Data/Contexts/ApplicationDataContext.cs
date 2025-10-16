using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StockService.Domain.Entities;

namespace StockService.Infrastructure.Data.Contexts;

public class ApplicationDataContext : DbContext
{
    public DbSet<Product> Products { get; set; } = null!;
    
    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}