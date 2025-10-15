using SharedService.Shared;

namespace AuthService.Domain.ValueObjects;

public class Role : ValueObject
{
    public string Name { get; }
    public DateTime? ValidUntil { get; private set; }

    private Role(string name, DateTime? validUntil = null)
    {
        Name = name.ToUpper();
        ValidUntil = validUntil;
    }

    public static Result<Role> Create(string roleName, DateTime? validUntil = null)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return Result<Role>.Fail("O nome da role é necessária");
        
        var role = new Role(roleName, validUntil);

        return Result<Role>.Ok(role);
    }

    public bool IsActive() 
        => !ValidUntil.HasValue || ValidUntil.Value >= DateTime.UtcNow;

    public void ExtendValidity(DateTime newExpiry)
    {
        if (!ValidUntil.HasValue || newExpiry > ValidUntil.Value)
            ValidUntil = newExpiry;
    }
}