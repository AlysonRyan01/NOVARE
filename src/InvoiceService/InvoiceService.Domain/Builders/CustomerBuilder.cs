using InvoiceService.Domain.Contracts;
using InvoiceService.Domain.Entities;
using InvoiceService.Domain.ValueObjects.Customers;
using SharedService.Shared;

namespace InvoiceService.Domain.Builders;

public class CustomerBuilder : IBuilder<Customer>
{
    private string _name = null!;
    private string _email = null!;
    private string _phone = null!;
    private string _document = null!;

    public CustomerBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public CustomerBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public CustomerBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    public CustomerBuilder WithDocument(string document)
    {
        _document = document;
        return this;
    }
    
    public Result<Customer> Build()
    {
        var nameResult = Name.Create(_name);
        var emailResult = Email.Create(_email);
        var documentResult = Document.Create(_document);
        var phoneResult = Phone.Create(_phone);
        
        var errors = new List<string>();
        
        if (!nameResult.IsSuccess)
            errors.AddRange(nameResult.Errors!);
            
        if (!emailResult.IsSuccess)
            errors.AddRange(emailResult.Errors!);
            
        if (!documentResult.IsSuccess)
            errors.AddRange(documentResult.Errors!);
            
        if (!phoneResult.IsSuccess)
            errors.AddRange(phoneResult.Errors!);
        
        if (errors.Any())
            return Result<Customer>.Fail(errors);
        
        var customer = new Customer(
            nameResult.Value!,
            emailResult.Value!,
            documentResult.Value!,
            phoneResult.Value!
        );

        return Result<Customer>.Ok(customer);
    }
}