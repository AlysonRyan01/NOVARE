using Gateway.Api.Endpoints;
using Gateway.Api.Middlewares;

namespace Gateway.Api.Extensions;

public static class AppExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapStockEndpoints();
        app.MapInvoiceGatewayEndpoints();
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
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
    }

    public static void AddCorsPolicy(this WebApplication app, WebApplicationBuilder builder )
    {
        var corsPolicyName = builder.Configuration["Cors:PolicyName"];
        app.UseCors(corsPolicyName!);
    }
}