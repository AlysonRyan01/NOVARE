using CustomerService.Application.Mappers;
using CustomerService.Domain.Entities;
namespace CustomerService.Tests.Application.Mappers;

[TestClass]
public class CustomerMapperTests
{
    [TestMethod]
    public void ToDto_Should_Map_SingleCustomer_ToCustomerDto()
    {
        var customer = Customer.Create(
            name: "Alyson Ryan",
            email: "alyson@example.com",
            phone: "11987654321",
            document: "12345678901"
        ).Value!; 
        
        var dto = customer.ToDto();
        
        Assert.IsNotNull(dto);
        Assert.AreEqual(customer.Id, dto.Id);
        Assert.AreEqual(customer.Name, dto.Name);
        Assert.AreEqual(customer.Email, dto.Email);
        Assert.AreEqual(customer.Phone, dto.Phone);
        Assert.AreEqual(customer.Document, dto.Document);
    }

    [TestMethod]
    public void ToDto_Should_Map_CustomerList_ToCustomerDtoList()
    {
        var customer1 = Customer.Create(
            name: "Alyson Ryan",
            email: "alyson@example.com",
            phone: "11987654321",
            document: "12345678901"
        ).Value!;

        var customer2 = Customer.Create(
            name: "John Doe",
            email: "john@example.com",
            phone: "11912345678",
            document: "98765432100"
        ).Value!;

        var customers = new List<Customer> { customer1, customer2 };
        
        var dtos = customers.ToDto();
        
        Assert.IsNotNull(dtos);
        Assert.HasCount(2, dtos);

        Assert.AreEqual(customer1.Id, dtos[0].Id);
        Assert.AreEqual(customer2.Id, dtos[1].Id);

        Assert.AreEqual(customer1.Name, dtos[0].Name);
        Assert.AreEqual(customer2.Name, dtos[1].Name);
    }
}