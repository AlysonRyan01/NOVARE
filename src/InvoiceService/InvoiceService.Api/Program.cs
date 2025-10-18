using InvoiceService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddPostgreSql();
builder.AddSignalR();
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
app.MapEndpoints();
app.AddSignalR();
app.AddSwagger();

app.Run();