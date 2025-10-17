using Gateway.Api.Middlewares;

namespace Gateway.Api.Extensions;

public static class AppExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapStockEndpoints();
        app.MapInvoiceEndpoint();
    }
    
    public static void AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    public static void AddCustomMiddlewares(this WebApplication app)
    {
        app.UseMiddleware<CookieToHeaderMiddleware>();
    }

    public static void AddHeaderPropagation(this WebApplication app)
    {
        app.UseHeaderPropagation();
    }

    public static void AddCorsPolicy(this WebApplication app, WebApplicationBuilder builder )
    {
        var corsPolicyName = builder.Configuration["Cors:PolicyName"];
        app.UseCors(corsPolicyName!);
    }
    
    public static void AddAuthorization(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}