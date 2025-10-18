using Gateway.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddSwagger();
builder.AddCorsConfiguration();
builder.AddHttpClients();
builder.AddDependencies();

var app = builder.Build();

app.AddSwagger();
app.AddCorsPolicy(builder);
app.AddCustomMiddlewares();
app.MapEndpoints();     

app.Run();
