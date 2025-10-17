namespace Gateway.Api.Middlewares;

public class CookieToHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public CookieToHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Cookies.TryGetValue("access_token", out var token))
        {
            context.Request.Headers["Authorization"] = $"Bearer {token}";
        }

        await _next(context);
    }
}