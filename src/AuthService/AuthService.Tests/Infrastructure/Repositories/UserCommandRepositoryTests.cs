using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data.Contexts;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Infrastructure.Repositories;

[TestClass]
public class UserCommandRepositoryTests
{
    private UserCommandRepository _repository = null!;
    private ApplicationDataContext _context = null!;

    private void SetupInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDataContext(options);
        _repository = new UserCommandRepository(_context);
    }

    [TestMethod]
    public async Task AddAsync_ShouldAddEnterprise()
    {
        SetupInMemoryDb();

        var enterpriseResult = User.Create("Empresa X", "email@x.com", "Senha123");
        Assert.IsTrue(enterpriseResult.IsSuccess);
        var enterprise = enterpriseResult.Value!;
        
        var result = await _repository.AddAsync(enterprise);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("Empresa X", result.Value!.Name.Value);
        Assert.AreEqual(1, _context.Users.Count());
    }
}