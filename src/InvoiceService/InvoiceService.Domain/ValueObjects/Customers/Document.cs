using SharedService.Shared;

namespace InvoiceService.Domain.ValueObjects.Customers;

public class Document
{
    public string Value { get; }

    private Document(string value) => Value = value;

    public static Result<Document> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Document>.Fail(["Documento é obrigatório"]);
        
        var cleanDocument = new string(value.Where(char.IsDigit).ToArray());
        
        if (cleanDocument.Length != 11 && cleanDocument.Length != 14)
            return Result<Document>.Fail(["Documento deve ter 11 dígitos (CPF) ou 14 dígitos (CNPJ)"]);

        return Result<Document>.Ok(new Document(cleanDocument));
    }

    public override string ToString() => Value;
}