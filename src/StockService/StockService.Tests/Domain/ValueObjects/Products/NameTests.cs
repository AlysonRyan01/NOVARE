using StockService.Domain.ValueObjects.Product;

namespace StockService.Tests.Domain.ValueObjects.Products;

[TestClass]
public class NameTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenValueIsValid()
    {
        var validName = "Produto A";
        
        var result = Name.Create(validName);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(validName, result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnFail_WhenValueIsNull()
    {
        string? invalidName = null;
        
        var result = Name.Create(invalidName!);
        
        Assert.IsFalse(result.IsSuccess);
        CollectionAssert.Contains(result.Errors!.ToList(), "O nome é obrigatório");
    }

    [TestMethod]
    public void Create_ShouldReturnFail_WhenValueIsEmpty()
    {
        var invalidName = "";
        
        var result = Name.Create(invalidName);
        
        Assert.IsFalse(result.IsSuccess);
        CollectionAssert.Contains(result.Errors!.ToList(), "O nome é obrigatório");
    }

    [TestMethod]
    public void ToString_ShouldReturnValue()
    {
        var validName = "Produto B";
        var nameResult = Name.Create(validName);
        var name = nameResult.Value!;
        
        var stringValue = name.ToString();

        Assert.AreEqual(validName, stringValue);
    }
}