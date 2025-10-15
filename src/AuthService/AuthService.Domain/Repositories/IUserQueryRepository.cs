using AuthService.Domain.Entities;
using AuthService.Domain.ValueObjects;
using SharedService.Shared;

namespace AuthService.Domain.Repositories;

public interface IUserQueryRepository
{
    Task<Result<User?>> GetByIdAsync(Guid id);
    Task<Result<User?>> GetByEmailAsync(string email);
    Task<Result<IEnumerable<Role>>> GetRolesAsync(Guid id);
}