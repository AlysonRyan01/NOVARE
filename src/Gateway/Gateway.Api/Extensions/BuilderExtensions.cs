using Gateway.Api.Consumers;
using Gateway.Api.Interfaces;
using Gateway.Api.Services;
using MassTransit;
using Microsoft.OpenApi.Models;
using SharedService.Shared.Events;
using SharedService.Shared.Settings;

namespace Gateway.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IStockService, StockService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();
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
        
        builder.Services.AddHttpClient("CustomerService", client =>
        {
            client.BaseAddress = new Uri("http://customerservice.api:5000");
        });
    }
    
    public static void AddSignalR(this WebApplicationBuilder builder)
    {
        builder.Services.AddSignalR();
    }
    
    public static void AddRabbitMq(this WebApplicationBuilder builder)
    {
        var rabbitMqSettings = builder.Configuration
            .GetSection("RabbitMqSettings")
            .Get<RabbitMqSettings>();
        
        if (rabbitMqSettings is null)
            throw new ArgumentNullException(nameof(rabbitMqSettings));
        
        builder.Services.AddMassTransit(busConfiguration =>
        {
            busConfiguration.AddConsumer<OutOfStockEventConsumer>();
            busConfiguration.AddConsumer<StockReservedEventConsumer>();
            
            busConfiguration.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqSettings.ConnectionString), host =>
                {
                    host.Username(rabbitMqSettings.User);
                    host.Password(rabbitMqSettings.Password);
                });
                
                cfg.ConfigureEndpoints(ctx);
            });
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
                    Title = "Gateway API",
                    Description = "Serviço de redirecionamento",
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