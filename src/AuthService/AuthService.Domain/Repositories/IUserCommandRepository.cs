using AuthService.Domain.Entities;
using SharedService.Shared;

namespace AuthService.Domain.Repositories;

public interface IUserCommandRepository
{
    Task<Result<User?>> AddAsync(User user);
}