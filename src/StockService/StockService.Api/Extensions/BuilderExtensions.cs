using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductService.Application.Consumers;
using ProductService.Application.Interfaces;
using ProductService.Infrastructure.EventPublishers;
using SharedService.Shared.Settings;
using Stock.Infrastructure.Repositories;
using StockService.Application.Commands;
using StockService.Application.Services;
using StockService.Application.Validations.Commands;
using StockService.Domain.Repositories;
using StockService.Infrastructure.Data.Contexts;
using StockService.Infrastructure.Services;

namespace StockService.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddPostgreSql(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'StockSqlConnection' não encontrada.");

        builder.Services.AddDbContext<ApplicationDataContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("StockService.Infrastructure"))
        );
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
            busConfiguration.AddConsumer<VerifyProductsStockConsumer>();
            
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

    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IStockReservationService, StockReservationService>();
        builder.Services.AddScoped<IEventPublisher, EventPublisher>();
    }

    public static void AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly);
        });
    }

    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IProductCommandRepository, ProductCommandRepository>();
        builder.Services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
    }

    public static void AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<CreateProductCommandValidator>();
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
                    Title = "Serviço de estoque",
                    Description = "Serviço de estoque",
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