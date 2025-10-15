using System.Reflection;
using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Data.Contexts;

public class ApplicationDataContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    
    public ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}