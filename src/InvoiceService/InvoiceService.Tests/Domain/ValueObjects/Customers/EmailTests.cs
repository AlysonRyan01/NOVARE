using InvoiceService.Domain.ValueObjects.Customers;

namespace InvoiceService.Tests.Domain.ValueObjects.Customers;

[TestClass]
public class EmailTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_ForValidEmail()
    {
        var email = "Test.Email@Example.com";
        
        var result = Email.Create(email);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("test.email@example.com", result.Value!.Value); // deve trim e lowercase
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForEmptyEmail()
    {
        var email = "";
        
        var result = Email.Create(email);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Email é obrigatório"));
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForInvalidFormatEmail()
    {
        var email = "invalid-email@";
        
        var result = Email.Create(email);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Email inválido"));
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForTooLongEmail()
    {
        var email = new string('a', 250) + "@example.com"; // > 255 chars

        var result = Email.Create(email);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Email não pode exceder 255 caracteres"));
    }

    [TestMethod]
    public void ToString_ShouldReturnValue()
    {
        var email = "user@example.com";
        var result = Email.Create(email);

        var emailString = result.Value!.ToString();

        Assert.AreEqual("user@example.com", emailString);
    }
}