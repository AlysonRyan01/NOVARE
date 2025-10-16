using AuthService.Domain.ValueObjects;
using SharedService.Shared;

namespace AuthService.Domain.Entities;

public class User : Entity
{
    public Name? Name { get; private set; }
    public Email Email { get; set; } = null!;
    public PasswordHash Password { get; private set; } = null!;
    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

    protected User() { }

    private User(
        Email email,
        PasswordHash password,
        IEnumerable<Role>? roles = null)
    {
        Email = email;
        Password = password;
        if (roles != null)
            _roles.AddRange(roles);
    }
    
    public static Result<User> Create(
        string email,
        string passwordHash,
        IEnumerable<Role>? roles = null)
    {
        var emailResult = Email.Create(email);
        if (!emailResult.IsSuccess)
            return Result<User>.Fail(emailResult.Errors!);

        var passwordResult = PasswordHash.Create(passwordHash);
        if (!passwordResult.IsSuccess)
            return Result<User>.Fail(passwordResult.Errors!);
        
        var user = new User(
            emailResult.Value!,
            passwordResult.Value!,
            roles
        );

        return Result<User>.Ok(user);
    }

    public Result<User> SetName(Name name)
    {
        Name = name;
        
        return Result<User>.Ok(this);
    }
}