using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AuthService.Infrastructure.Data.Contexts;

public class ApplicationDataContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
{
    public ApplicationDataContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly("AuthService.Infrastructure")
        );

        return new ApplicationDataContext(optionsBuilder.Options);
    }
}