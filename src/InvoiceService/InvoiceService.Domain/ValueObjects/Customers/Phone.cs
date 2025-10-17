using SharedService.Shared;

namespace InvoiceService.Domain.ValueObjects.Customers;

public class Phone
{
    public string Value { get; }

    private Phone(string value) => Value = value;

    public static Result<Phone> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Phone>.Ok(new Phone(string.Empty));
        
        var cleanPhone = new string(value.Where(char.IsDigit).ToArray());

        if (cleanPhone.Length < 10)
            return Result<Phone>.Fail(["Telefone deve ter pelo menos 10 dígitos"]);

        if (cleanPhone.Length > 15)
            return Result<Phone>.Fail(["Telefone não pode exceder 15 dígitos"]);

        return Result<Phone>.Ok(new Phone(cleanPhone));
    }
    
    public override string ToString() => Value;
}