using SharedService.Shared;

namespace InvoiceService.Domain.ValueObjects.Customers;

public class Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Fail(["Email é obrigatório"]);

        value = value.Trim().ToLower();

        if (value.Length > 255)
            return Result<Email>.Fail(["Email não pode exceder 255 caracteres"]);

        try
        {
            var mailAddress = new System.Net.Mail.MailAddress(value);
            if (mailAddress.Address != value)
                return Result<Email>.Fail(["Email inválido"]);

            return Result<Email>.Ok(new Email(value));
        }
        catch
        {
            return Result<Email>.Fail(["Email inválido"]);
        }
    }

    public override string ToString() => Value;
}