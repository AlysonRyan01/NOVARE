using StockService.Domain.ValueObjects.Product;

namespace StockService.Tests.Domain.ValueObjects.Products;

[TestClass]
public class PriceTests
{
    [TestMethod]
    public void Create_ShouldReturnSuccess_WhenValueIsPositive()
    {
        var validPrice = 100.50m;
        
        var result = Price.Create(validPrice);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(validPrice, result.Value!.Value);
    }

    [TestMethod]
    public void Create_ShouldReturnFail_WhenValueIsZero()
    {
        var invalidPrice = 0m;
        
        var result = Price.Create(invalidPrice);
        
        Assert.IsFalse(result.IsSuccess);
        CollectionAssert.Contains(result.Errors!.ToList(), "O preço deve ser maior que 0");
    }

    [TestMethod]
    public void Create_ShouldReturnFail_WhenValueIsNegative()
    {
        var invalidPrice = -50m;
        
        var result = Price.Create(invalidPrice);
        
        Assert.IsFalse(result.IsSuccess);
        CollectionAssert.Contains(result.Errors!.ToList(), "O preço deve ser maior que 0");
    }

    [TestMethod]
    public void ToString_ShouldReturnFormattedCurrency()
    {
        var priceValue = 123.45m;
        var priceResult = Price.Create(priceValue);
        var price = priceResult.Value!;

        var stringValue = price.ToString();
        
        Assert.AreEqual(priceValue.ToString("C"), stringValue);
    }
}