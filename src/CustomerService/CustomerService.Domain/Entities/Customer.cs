using CustomerService.Domain.Abstractions;
using CustomerService.Domain.Validations;
using SharedService.Shared;

namespace CustomerService.Domain.Entities;

public class Customer : Entity
{
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public string Document { get; private set; } = null!;

    protected Customer() { }

    private Customer(
        string name, 
        string email, 
        string phone, 
        string document)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Phone = phone;
        Document = document;
    }

    public static Result<Customer> Create(
        string name, 
        string email, 
        string phone, 
        string document)
    {
        var customer = new Customer(name, email, phone, document);
        return customer.CreateValidator();
    }
}