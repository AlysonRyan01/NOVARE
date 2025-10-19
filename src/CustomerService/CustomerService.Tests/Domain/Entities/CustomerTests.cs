using CustomerService.Domain.Entities;
namespace CustomerService.Tests.Domain.Entities;

[TestClass]
public class CustomerTests
{
    [TestMethod]
    public void CreateCustomer_ShouldReturnOk_WhenDataIsValid()
    {
        string name = "Alyson Ryan";
        string email = "alyson@example.com";
        string phone = "11987654321";
        string document = "12345678901";
        
        var result = Customer.Create(name, email, phone, document);
        
        Assert.IsTrue(result.IsSuccess, "Expected creation to succeed with valid data.");
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(name, result.Value.Name);
        Assert.AreEqual(email, result.Value.Email);
        Assert.AreEqual(phone, result.Value.Phone);
        Assert.AreEqual(document, result.Value.Document);
    }

    [TestMethod]
    public void CreateCustomer_ShouldFail_WhenNameIsEmpty()
    {
        string name = "";
        string email = "alyson@example.com";
        string phone = "11987654321";
        string document = "12345678901";
        
        var result = Customer.Create(name, email, phone, document);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O nome precisa ter entre 1 e 100 caracteres."));
    }

    [TestMethod]
    public void CreateCustomer_ShouldFail_WhenEmailIsInvalid()
    {
        string name = "Alyson";
        string email = "invalid-email";
        string phone = "11987654321";
        string document = "12345678901";
        
        var result = Customer.Create(name, email, phone, document);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Email inválido."));
    }

    [TestMethod]
    public void CreateCustomer_ShouldFail_WhenPhoneIsInvalid()
    {
        string name = "Alyson";
        string email = "alyson@example.com";
        string phone = "123";
        string document = "12345678901";
        
        var result = Customer.Create(name, email, phone, document);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Telefone inválido. Deve conter 10 ou 11 dígitos."));
    }

    [TestMethod]
    public void CreateCustomer_ShouldFail_WhenDocumentIsInvalid()
    {
        string name = "Alyson";
        string email = "alyson@example.com";
        string phone = "11987654321";
        string document = "123";
        
        var result = Customer.Create(name, email, phone, document);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Documento inválido. Deve conter 11 ou 14 dígitos."));
    }
}