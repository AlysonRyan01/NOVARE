using AuthService.Application.Commands;
using MediatR;

namespace AuthService.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("api/auth", async (HttpContext context, ISender handler, AuthenticateUserCommand command) =>
        {
            var result = await handler.Send(command);
            if (!result.IsSuccess)
                return Results.BadRequest(result);
            
            return Results.Ok(result);
        });
    }
}