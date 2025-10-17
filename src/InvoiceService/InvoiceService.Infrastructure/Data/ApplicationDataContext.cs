using System.Reflection;
using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InvoiceService.Infrastructure.Data;

public class ApplicationDataContext : DbContext
{
    public DbSet<Invoice> Invoices { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<InvoiceItem> InvoiceItems { get; set; } = null!;
    
    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}