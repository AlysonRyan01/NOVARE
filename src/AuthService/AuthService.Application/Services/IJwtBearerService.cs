using AuthService.Domain.Entities;

namespace AuthService.Application.Services;

public interface IJwtBearerService
{
    Task<string> Generate(User user);
}