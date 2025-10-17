using InvoiceService.Domain.Entities;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Mappers.Customers;

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto(
            Id: customer.Id,
            Name: customer.Name.Value,
            Email: customer.Email.Value,
            Phone: customer.Phone.Value,
            Document: customer.Document.Value
        );
    }

    public static List<CustomerDto> ToDto(this IEnumerable<Customer> customers)
    {
        return customers.Select(customer => customer.ToDto()).ToList();
    }
}