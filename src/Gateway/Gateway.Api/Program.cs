using Gateway.Api.Extensions;
using Gateway.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.AddSwagger();
builder.AddCorsConfiguration();
builder.AddHeaderPropagation();
builder.AddAuthentication();
builder.AddHttpClients();
builder.AddDependencies();

builder.Services.AddTransient<CookieToHeaderMiddleware>();

var app = builder.Build();

app.AddSwagger();
app.AddCorsPolicy(builder);
app.AddCustomMiddlewares();
app.AddHeaderPropagation();
app.AddAuthorization();
app.MapEndpoints();     

app.Run();
