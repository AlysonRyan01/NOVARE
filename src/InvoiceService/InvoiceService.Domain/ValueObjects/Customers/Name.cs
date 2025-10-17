using SharedService.Shared;

namespace InvoiceService.Domain.ValueObjects.Customers;

public class Name
{
    public string Value { get; }

    private Name(string value) => Value = value;

    public static Result<Name> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Name>.Fail(["Nome é obrigatório"]);

        value = value.Trim();

        if (value.Length < 2)
            return Result<Name>.Fail(["Nome deve ter pelo menos 2 caracteres"]);

        if (value.Length > 100)
            return Result<Name>.Fail(["Nome não pode exceder 100 caracteres"]);

        return Result<Name>.Ok(new Name(value));
    }

    public override string ToString() => Value;
}