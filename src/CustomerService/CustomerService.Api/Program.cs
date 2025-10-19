using CustomerService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddPostgreSql();
builder.AddDependencies();
builder.AddMediatR();
builder.AddRepositories();
builder.AddFluentValidation();
builder.AddCorsConfiguration();
builder.AddSwagger();

var app = builder.Build();
app.AddExceptionsMiddleware();
app.ApplyMigrations();
app.AddCorsPolicy(builder);
app.MapEndpoints();
app.AddSwagger();

app.Run();