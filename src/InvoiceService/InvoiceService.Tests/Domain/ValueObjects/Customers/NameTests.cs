using InvoiceService.Domain.ValueObjects.Customers;

namespace InvoiceService.Tests.Domain.ValueObjects.Customers;

[TestClass]
public class NameTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_ForValidName()
    {
        var name = "João da Silva";
        
        var result = Name.Create(name);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("João da Silva", result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForEmptyName()
    {
        var name = "";

        var result = Name.Create(name);

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Nome é obrigatório"));
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForTooShortName()
    {
        var name = "J"; // menos de 2 caracteres
        
        var result = Name.Create(name);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Nome deve ter pelo menos 2 caracteres"));
    }

    [TestMethod]
    public void Create_ShouldReturnFail_ForTooLongName()
    {
        var name = new string('a', 101); // mais de 100 caracteres
        
        var result = Name.Create(name);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Nome não pode exceder 100 caracteres"));
    }

    [TestMethod]
    public void ToString_ShouldReturnValue()
    {
        var name = "Maria Oliveira";
        var result = Name.Create(name);
        
        var nameString = result.Value!.ToString();
        
        Assert.AreEqual("Maria Oliveira", nameString);
    }
}