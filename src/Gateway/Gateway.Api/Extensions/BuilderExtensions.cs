using Gateway.Api.Interfaces;
using Gateway.Api.Services;
using Microsoft.OpenApi.Models;

namespace Gateway.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStockService, StockService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
    }

    public static void AddHttpClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient("StockService", client =>
        {
            client.BaseAddress = new Uri("http://stockservice.api:5000");
        });

        builder.Services.AddHttpClient("InvoiceService", client =>
        {
            client.BaseAddress = new Uri("http://invoiceservice.api:5000");
        });
    }
    
    public static void AddSwagger(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddEndpointsApiExplorer(); 

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = "Serviço de autenticação",
                    Description = "Serviço de autenticação com JWT Bearer",
                    Version = "v1",
                    Contact = new OpenApiContact { Name = "Alyson Ryan Ullirsch", Email = "alysonullirsch8@gmail.com" }
                });
            });
        }
    }
    
    public static void AddCorsConfiguration(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        
        var policyName = configuration["Cors:PolicyName"];
        var origins = configuration["Cors:Origins"]?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(policyName!, policy =>
            {
                policy
                    .WithOrigins(origins!)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }
}