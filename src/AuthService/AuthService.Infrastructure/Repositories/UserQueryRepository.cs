using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Domain.ValueObjects;
using AuthService.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedService.Shared;

namespace AuthService.Infrastructure.Repositories;

public class UserQueryRepository : IUserQueryRepository
{
    private readonly ApplicationDataContext _context;

    public UserQueryRepository(ApplicationDataContext context)
    {
        _context = context;
    }
    
    public async Task<Result<User?>> GetByIdAsync(Guid id)
    {
        var user = await _context
            .Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (user == null)
            return Result<User?>.Fail(["Usuário nao encontrado"]);
        
        return Result<User?>.Ok(user);
    }

    public async Task<Result<User?>> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return Result<User?>.Fail(["E-mail inválido"]);

        var normalizedEmail = email.Trim().ToLower();

        var user = await _context
            .Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email.Value.ToLower() == normalizedEmail);

        if (user == null)
            return Result<User?>.Fail(["Usuário nao encontrado"]);

        return Result<User?>.Ok(user);
    }

    public async Task<Result<IEnumerable<Role>>> GetRolesAsync(Guid id)
    {
        var user = await _context
            .Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .FirstOrDefaultAsync(x => x.Id == id);
        
        if (user == null)
            return Result<IEnumerable<Role>>.Fail(["Usuário nao encontrado"]);
        
        return Result<IEnumerable<Role>>.Ok(user.Roles);
    }
}