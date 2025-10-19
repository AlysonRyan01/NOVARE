using System.Text.Json;
using SharedService.Shared;

namespace CustomerService.Api.Middlewares;

public class ExceptionsHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionsHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionsHandlerMiddleware(
        RequestDelegate next, 
        ILogger<ExceptionsHandlerMiddleware> logger, 
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu uma exceção: {Message}", ex.Message);

            context.Response.ContentType = "application/json";
            string message = _env.IsDevelopment()
                ? $"Erro inesperado: {ex.Message}\n{ex.StackTrace}"
                : "Ocorreu um erro interno no servidor, tente novamente mais tarde.";

            var response = new BaseResponse<object>(null, 500, message);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}