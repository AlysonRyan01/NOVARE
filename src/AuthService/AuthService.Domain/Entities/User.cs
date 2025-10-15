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
        Name name,
        Email email,
        PasswordHash password,
        IEnumerable<Role>? roles = null)
    {
        Name = name;
        Email = email;
        Password = password;
        if (roles != null)
            _roles.AddRange(roles);
    }
    
    public static Result<User> Create(
        string name,
        string email,
        string passwordHash,
        IEnumerable<Role>? roles = null)
    {
        var nameResult = Name.Create(name);
        if (!nameResult.IsSuccess)
            return Result<User>.Fail(nameResult.Error!);

        var emailResult = Email.Create(email);
        if (!emailResult.IsSuccess)
            return Result<User>.Fail(emailResult.Error!);

        var passwordResult = PasswordHash.Create(passwordHash);
        if (!passwordResult.IsSuccess)
            return Result<User>.Fail(passwordResult.Error!);
        
        var user = new User(
            nameResult.Value!,
            emailResult.Value!,
            passwordResult.Value!,
            roles
        );

        return Result<User>.Ok(user);
    }
}