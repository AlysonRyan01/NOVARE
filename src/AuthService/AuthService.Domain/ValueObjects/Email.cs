using System.Text.RegularExpressions;
using SharedService.Shared;

namespace AuthService.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; private set; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !EmailRegex.IsMatch(value))
            return Result<Email>.Fail(["Forneça um email válido"]);

        var email = new Email(value);
        
        return Result<Email>.Ok(email);
    }

    public override string ToString()
    {
        return Value;
    }
    
    private static readonly Regex EmailRegex = new Regex(
        @"^[^\s@]+@[^\s@]+\.[^\s@]+$", 
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
}