using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Data.Contexts;
using AuthService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Tests.Infrastructure.Repositories;

[TestClass]
public class UserQueryRepositoryTests
{
    private UserQueryRepository _repository = null!;
    private ApplicationDataContext _context = null!;

    private void SetupInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDataContext(options);
        _repository = new UserQueryRepository(_context);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldReturnEnterprise_WhenExists()
    {
        SetupInMemoryDb();

        var enterprise = User.Create("email@a.com", "Senha123").Value!;
        await _context.Users.AddAsync(enterprise);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetByIdAsync(enterprise.Id);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
    }

    [TestMethod]
    public async Task GetByIdAsync_ShouldFail_WhenNotFound()
    {
        SetupInMemoryDb();
        var nonExistentId = Guid.NewGuid();
        
        var result = await _repository.GetByIdAsync(nonExistentId);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Usuário nao encontrado", result.Errors!.FirstOrDefault());
    }

    [TestMethod]
    public async Task GetByEmailAsync_ShouldReturnEnterprise_WhenExists()
    {
        SetupInMemoryDb();

        var enterprise = User.Create("email@b.com", "Senha123").Value!;
        await _context.Users.AddAsync(enterprise);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetByEmailAsync("EMAIL@B.COM"); 
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual("email@b.com", result.Value!.Email.Value);
    }

    [TestMethod]
    public async Task GetByEmailAsync_ShouldFail_WhenNotFound()
    {
        SetupInMemoryDb();
        
        var result = await _repository.GetByEmailAsync("notfound@email.com");
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Usuário nao encontrado", result.Errors!.FirstOrDefault());
    }

    [TestMethod]
    public async Task GetRolesAsync_ShouldReturnRoles_WhenExists()
    {
        SetupInMemoryDb();

        var role1 = Role.Create("Admin").Value!;
        var role2 = Role.Create("User").Value!;
        var roles = new[] { role1, role2 };

        var enterprise = User.Create("email@c.com", "Senha123", roles).Value!;
        await _context.Users.AddAsync(enterprise);
        await _context.SaveChangesAsync();
        
        var result = await _repository.GetRolesAsync(enterprise.Id);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(2, result.Value!.Count());
        Assert.IsTrue(result.Value!.Any(r => r.Name == "ADMIN"));
        Assert.IsTrue(result.Value!.Any(r => r.Name == "USER"));
    }

    [TestMethod]
    public async Task GetRolesAsync_ShouldFail_WhenEnterpriseNotFound()
    {
        SetupInMemoryDb();
        var nonExistentId = Guid.NewGuid();
        
        var result = await _repository.GetRolesAsync(nonExistentId);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.AreEqual("Usuário nao encontrado", result.Errors!.FirstOrDefault());
    }
}