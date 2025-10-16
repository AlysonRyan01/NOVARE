using AuthService.Application.Commands;
using MediatR;

namespace AuthService.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth", async (HttpContext context, ISender handler, AuthenticateUserCommand command) =>
        {
            var result = await handler.Send(command);
            if (!result.IsSuccess)
                return Results.BadRequest(result);
            
            context.Response.Cookies.Append("access_token", result.Value!, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(30),
                Path = "/"
            });
            
            return Results.Ok(result);
        });
    }
}