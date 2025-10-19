using FluentValidation;
using InvoiceService.Application.Commands.Invoices;
using InvoiceService.Application.Services;
using InvoiceService.Application.Validations.Commands.Invoices;
using InvoiceService.Domain.Repositories.Invoices;
using InvoiceService.Infrastructure.Consumers;
using InvoiceService.Infrastructure.Data;
using InvoiceService.Infrastructure.Repositories.Invoices;
using InvoiceService.Infrastructure.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharedService.Shared.Settings;

namespace InvoiceService.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddPostgreSql(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'StockSqlConnection' não encontrada.");

        builder.Services.AddDbContext<ApplicationDataContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("InvoiceService.Infrastructure"))
        );
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
            busConfiguration.AddConsumer<OutOfStockConsumer>();
            busConfiguration.AddConsumer<StockReservedConsumer>();
            
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
        builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
    }

    public static void AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(typeof(CreateInvoiceCommand).Assembly);
        });
    }

    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IInvoiceCommandRepository, InvoiceCommandRepository>();
        builder.Services.AddScoped<IInvoiceQueryRepository, InvoiceQueryRepository>();
    }

    public static void AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<CreateInvoiceCommandValidator>();
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