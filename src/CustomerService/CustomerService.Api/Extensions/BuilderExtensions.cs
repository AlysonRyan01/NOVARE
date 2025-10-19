using CustomerService.Application.Commands;
using CustomerService.Application.Services;
using CustomerService.Application.Validations;
using CustomerService.Domain.Repositories;
using CustomerService.Infrastructure.Data;
using CustomerService.Infrastructure.Repositories;
using CustomerService.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace CustomerService.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddPostgreSql(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'CustomerSQLConnection' não encontrada.");

        builder.Services.AddDbContext<ApplicationDataContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("CustomerService.Infrastructure"))
        );
    }

    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly);
        });
    }

    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICustomerCommandRepository, CustomerCommandRepository>();
        builder.Services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
    }

    public static void AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();
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
                    Title = "Serviço de clientes",
                    Description = "Serviço de clientes",
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