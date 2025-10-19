using InvoiceService.Domain.ValueObjects.Invoices;

namespace InvoiceService.Tests.Domain.ValueObjects.Invoices;

[TestClass]
public class NumberTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_ForValidNumber()
    {
        var value = "NF123456";
        
        var result = Number.Create(value);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("NF123456", result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForEmptyNumber()
    {
        var value = "";
        
        var result = Number.Create(value);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O número da nota fiscal precisa ter um valor"));
    }

    [TestMethod]
    public void ToString_ShouldReturnValue()
    {
        var value = "NF987654";
        var result = Number.Create(value);
        
        var numberString = result.Value!.ToString();
        
        Assert.AreEqual("NF987654", numberString);
    }
}