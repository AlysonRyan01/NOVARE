using SharedService.Shared;

namespace AuthService.Domain.ValueObjects;

public class Name
{
    public string Value { get; private set; }

    private Name(string value)
    {
        Value = value;
    }

    public static Result<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Name>.Fail("A empresa precisa ter um nome");

        var name = new Name(value);
        
        return Result<Name>.Ok(name);
    }
    
    public override string ToString()
        => Value;
}