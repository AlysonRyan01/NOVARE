using System.Text;
using AuthService.Application.Commands;
using AuthService.Application.Commands.Validations;
using AuthService.Application.Services;
using AuthService.Domain.Repositories;
using AuthService.Infrastructure.Data.Contexts;
using AuthService.Infrastructure.Repositories;
using AuthService.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AuthService.Api.Extensions;

public static class BuilderExtensions
{
    public static void AddPostgreSql(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string 'AuthSqlConnection' não encontrada.");

        builder.Services.AddDbContext<ApplicationDataContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly("AuthService.Infrastructure"))
        );
    }
    
    public static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["JwtBearerSettings:SecretKey"]!)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            x.RequireHttpsMetadata = false;
            
            x.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.TryGetValue("access_token", out var token))
                    {
                        context.Token = token;
                    }
                    return Task.CompletedTask;
                }
            };
        });

        builder.Services.AddAuthorization();
    }

    public static void AddDependencies(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        builder.Services.AddScoped<IJwtBearerService, JwtBearerService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    }

    public static void AddMediatR(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(x =>
        {
            x.RegisterServicesFromAssembly(typeof(AuthenticateUserCommand).Assembly);
        });
    }

    public static void AddRepositories(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IUserCommandRepository, UserCommandRepository>();
        builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
    }

    public static void AddFluentValidation(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<AuthenticateUserCommandValidation>();
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