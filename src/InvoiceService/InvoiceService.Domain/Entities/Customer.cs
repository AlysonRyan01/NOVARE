using InvoiceService.Domain.Abstractions;
using InvoiceService.Domain.ValueObjects.Customers;

namespace InvoiceService.Domain.Entities;

public class Customer : Entity
{
    public Name Name { get; internal set; } = null!;
    public Email Email { get; internal set; } = null!;
    public Phone Phone { get; internal set; } = null!;
    public Document Document { get; internal set; } = null!;

    protected Customer() { }
    
    internal Customer(Name name, Email email, Document document, Phone phone)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        Document = document;
        Phone = phone;
    }
}