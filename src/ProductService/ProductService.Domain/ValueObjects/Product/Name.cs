using SharedService.Shared;

namespace ProductService.Domain.ValueObjects.Product;

public class Name
{
    public string Value { get; }

    private Name(string value) => Value = value;

    public static Result<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Name>.Fail(["O nome é obrigatório"]);

        var name = new Name(value);
        
        return Result<Name>.Ok(name);
    }

    public override string ToString() => Value;
}