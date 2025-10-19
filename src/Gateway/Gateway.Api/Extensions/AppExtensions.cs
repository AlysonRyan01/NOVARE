using Gateway.Api.Endpoints;
using Gateway.Api.Hubs;
using Gateway.Api.Middlewares;

namespace Gateway.Api.Extensions;

public static class AppExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapStockEndpoints();
        app.MapInvoiceGatewayEndpoints();
        app.MapCustomerGatewayEndpoints();
    }
    
    public static void AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
    
    public static void AddSignalR(this WebApplication app)
    {
        app.MapHub<GatewayHub>("/gatewayHub");
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