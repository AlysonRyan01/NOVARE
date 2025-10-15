using AuthService.Domain.Entities;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedService.Shared;

namespace AuthService.Infrastructure.Repositories;

public class UserCommandRepository : IUserCommandRepository
{
    private readonly ApplicationDataContext _context;

    public UserCommandRepository(ApplicationDataContext context)
    {
        _context = context;
    }
    
    public async Task<Result<User?>> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return Result<User?>.Ok(user);
    }
}