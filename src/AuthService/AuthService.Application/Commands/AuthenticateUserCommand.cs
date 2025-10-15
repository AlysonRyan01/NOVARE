using MediatR;
using SharedService.Shared;

namespace AuthService.Application.Commands;

public record AuthenticateUserCommand(string Email, string Password) : IRequest<Result<string>>;