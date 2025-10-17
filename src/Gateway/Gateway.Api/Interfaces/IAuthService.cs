using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace Gateway.Api.Interfaces;

public interface IAuthService
{
    Task<Result<string>> Authenticate(AuthenticateUserDto dto);
}