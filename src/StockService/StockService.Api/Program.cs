using StockService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddPostgreSql();
builder.AddDependencies();
builder.AddMediatR();
builder.AddRabbitMq();
builder.AddRepositories();
builder.AddFluentValidation();
builder.AddCorsConfiguration();
builder.AddSwagger();

var app = builder.Build();
app.ApplyMigrations();
app.AddExceptionsMiddleware();
app.AddCorsPolicy(builder);
app.MapEndpoints();
app.AddSwagger();

app.Run();