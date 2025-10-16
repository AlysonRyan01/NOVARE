using AuthService.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddAuthentication();
builder.AddPostgreSql();
builder.AddDependencies();
builder.AddMediatR();
builder.AddRepositories();
builder.AddFluentValidation();
builder.AddCorsConfiguration();
builder.AddSwagger();

var app = builder.Build();
app.AddAuthorization();
app.ApplyMigrations();
app.AddExceptionsMiddleware();
app.AddCorsPolicy(builder);
app.MapEndpoints();
app.AddSwagger();

app.Run();
