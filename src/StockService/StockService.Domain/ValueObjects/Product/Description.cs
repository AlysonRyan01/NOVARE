using SharedService.Shared;

namespace StockService.Domain.ValueObjects.Product;

public class Description
{
    public string Value { get; }

    private Description(string value) => Value = value;

    public static Result<Description> Create(string value)
    {
        if (value.Length < 10 || value.Length > 200)
            return Result<Description>.Fail(["A descrição deve ter entre 10 e 200 caracteres"]);

        var description = new Description(value);
        
        return Result<Description>.Ok(description);
    }

    public override string ToString() => Value;
}