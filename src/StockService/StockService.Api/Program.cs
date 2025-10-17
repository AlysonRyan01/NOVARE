using StockService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddAuthentication();
builder.AddPostgreSql();
builder.AddDependencies();
builder.AddMediatR();
builder.AddRabbitMq();
builder.AddRepositories();
builder.AddFluentValidation();
builder.AddCorsConfiguration();
builder.AddSwagger();

var app = builder.Build();
app.AddExceptionsMiddleware();
app.ApplyMigrations();
app.AddCorsPolicy(builder);
app.AddAuthentication();
app.MapEndpoints();
app.AddSwagger();

app.Run();