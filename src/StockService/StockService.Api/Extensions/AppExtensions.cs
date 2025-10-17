using Microsoft.EntityFrameworkCore;
using ProductService.Api.Endpoints;
using ProductService.Api.Middlewares;
using StockService.Infrastructure.Data.Contexts;

namespace StockService.Api.Extensions;

public static class AppExtensions
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapProductEndpoints();
    }
    
    public static void AddSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }

    public static void AddExceptionsMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionsHandlerMiddleware>();
    }
    
    public static void ApplyMigrations(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDataContext>();
            db.Database.Migrate();
        }
    }

    public static void AddCorsPolicy(this WebApplication app, WebApplicationBuilder builder )
    {
        var corsPolicyName = builder.Configuration["Cors:PolicyName"];
        app.UseCors(corsPolicyName!);
    }
}