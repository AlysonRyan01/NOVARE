using SharedService.Shared;

namespace AuthService.Domain.ValueObjects;

public class PasswordHash : ValueObject
{
    public string Value { get; private set; }

    private PasswordHash(string value)
    {
        Value = value;
    }

    public static Result<PasswordHash> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<PasswordHash>.Fail("A senha deve ser informada");

        var password = new PasswordHash(value);
        
        return Result<PasswordHash>.Ok(password);
    }

    public override string ToString()
    {
        return Value;
    }
}