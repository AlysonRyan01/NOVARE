using System.Text;
using Gateway.Api.Interfaces;
using Gateway.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Gateway.Api.Extensions;

public static class BuilderExtensions
{
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
        builder.Services.AddScoped<IStockService, StockService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
    }

    public static void AddHeaderPropagation(this WebApplicationBuilder builder)
    {
        builder.Services.AddHeaderPropagation(options =>
        {
            options.Headers.Add("Authorization");
        });
    }

    public static void AddHttpClients(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient("ProductService", client =>
        {
            client.BaseAddress = new Uri("");
        }).AddHeaderPropagation();
        
        builder.Services.AddHttpClient("StockService", client =>
        {
            client.BaseAddress = new Uri("");
        }).AddHeaderPropagation();
        
        builder.Services.AddHttpClient("AuthService", client =>
        {
            client.BaseAddress = new Uri("");
        }).AddHeaderPropagation();
        
        builder.Services.AddHttpClient("InvoiceService", client =>
        {
            client.BaseAddress = new Uri("");
        }).AddHeaderPropagation();
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