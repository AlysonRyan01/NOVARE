using System.Text.RegularExpressions;
using CustomerService.Domain.Entities;
using SharedService.Shared;

namespace CustomerService.Domain.Validations;

public static class CustomerValidations
{
    public static Result<Customer> CreateValidator(
        this Customer customer)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(customer.Name) || customer.Name.Length > 100)
            errors.Add("O nome precisa ter entre 1 e 100 caracteres.");

        if (string.IsNullOrWhiteSpace(customer.Email) || !Regex.IsMatch(customer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            errors.Add("Email inválido.");

        if (string.IsNullOrWhiteSpace(customer.Phone) || !Regex.IsMatch(customer.Phone, @"^\d{10,11}$"))
            errors.Add("Telefone inválido. Deve conter 10 ou 11 dígitos.");

        if (string.IsNullOrWhiteSpace(customer.Document) || !Regex.IsMatch(customer.Document, @"^(\d{11}|\d{14})$"))
            errors.Add("Documento inválido. Deve conter 11 ou 14 dígitos.");

        if (errors.Any())
            return Result<Customer>.Fail(errors);
        
        return Result<Customer>.Ok(customer);
    }
}