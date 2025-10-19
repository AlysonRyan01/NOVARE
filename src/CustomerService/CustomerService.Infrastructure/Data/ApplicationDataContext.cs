using System.Reflection;
using CustomerService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Infrastructure.Data;

public class ApplicationDataContext : DbContext
{
    public DbSet<Customer> Customers { get; set; } = null!;
    
    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}