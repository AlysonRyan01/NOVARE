using SharedService.Shared;

namespace InvoiceService.Domain.ValueObjects.Invoices;

public class Number
{
    public string Value { get; private set; }
    
    private Number(string value) => Value = value;

    public static Result<Number> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Number>.Fail(["O número da nota fiscal precisa ter um valor"]);

        var number = new Number(value);
        
        return Result<Number>.Ok(number);
    }

    public override string ToString() => Value;
}