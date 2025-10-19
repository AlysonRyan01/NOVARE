using CustomerService.Domain.Entities;
using SharedService.Shared.Dtos;

namespace CustomerService.Application.Mappers;

public static class CustomerMapper
{
    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto(
            Id: customer.Id,
            Name: customer.Name,
            Email: customer.Email,
            Phone: customer.Phone,
            Document: customer.Document
        );
    }

    public static List<CustomerDto> ToDto(this IEnumerable<Customer> customers)
    {
        return customers.Select(customer => customer.ToDto()).ToList();
    }
}