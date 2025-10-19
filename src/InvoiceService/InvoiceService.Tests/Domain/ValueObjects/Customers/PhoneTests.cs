using InvoiceService.Domain.ValueObjects.Customers;

namespace InvoiceService.Tests.Domain.ValueObjects.Customers;

[TestClass]
public class PhoneTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_ForValidPhone()
    {
        var phone = "(11) 91234-5678";
        
        var result = Phone.Create(phone);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("11912345678", result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnSuccess_ForEmptyPhone()
    {
        var phone = "";
        
        var result = Phone.Create(phone);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(string.Empty, result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForTooShortPhone()
    {
        var phone = "123456789";
        
        var result = Phone.Create(phone);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Telefone deve ter pelo menos 10 dígitos"));
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForTooLongPhone()
    {
        var phone = "1234567890123456"; 
        
        var result = Phone.Create(phone);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Telefone não pode exceder 15 dígitos"));
    }

    [TestMethod]
    public void ToString_ShouldReturnValue()
    {
        var phone = "(21) 99876-5432";
        var result = Phone.Create(phone);
        
        var phoneString = result.Value!.ToString();
        
        Assert.AreEqual("21998765432", phoneString);
    }
}