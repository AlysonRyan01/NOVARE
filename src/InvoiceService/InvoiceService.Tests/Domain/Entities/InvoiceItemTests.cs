using InvoiceService.Domain.Builders;

namespace InvoiceService.Tests.Domain.Entities;

[TestClass]
public class InvoiceItemTests
{
    [TestMethod]
    public void Build_ShouldReturnSuccess_WhenAllPropertiesAreValid()
    {
        var productId = Guid.NewGuid();
        var productName = "Produto Teste";
        var quantity = 5;
        var unitPrice = 10.5m;

        var builder = new InvoiceItemBuilder()
            .WithProductId(productId)
            .WithProductName(productName)
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice);

        var result = builder.Build();

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(productId, result.Value!.ProductId);
        Assert.AreEqual(productName, result.Value.ProductName);
        Assert.AreEqual(quantity, result.Value.Quantity);
        Assert.AreEqual(unitPrice, result.Value.UnitPrice);
        Assert.AreEqual(quantity * unitPrice, result.Value.TotalPrice);
    }

    [TestMethod]
    public void Build_ShouldReturnFail_WhenProductIdIsEmpty()
    {
        var builder = new InvoiceItemBuilder()
            .WithProductName("Produto")
            .WithQuantity(1)
            .WithUnitPrice(10);

        var result = builder.Build();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O ID do produto é obrigatório"));
    }

    [TestMethod]
    public void Build_ShouldReturnFail_WhenProductNameIsEmpty()
    {
        var builder = new InvoiceItemBuilder()
            .WithProductId(Guid.NewGuid())
            .WithQuantity(1)
            .WithUnitPrice(10);

        var result = builder.Build();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O nome do produto é obrigatório"));
    }

    [TestMethod]
    public void Build_ShouldReturnFail_WhenQuantityIsZeroOrNegative()
    {
        var builder = new InvoiceItemBuilder()
            .WithProductId(Guid.NewGuid())
            .WithProductName("Produto")
            .WithQuantity(0)
            .WithUnitPrice(10);

        var result = builder.Build();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("A quantidade deve ser maior que zero"));
    }

    [TestMethod]
    public void Build_ShouldReturnFail_WhenUnitPriceIsZeroOrNegative()
    {
        var builder = new InvoiceItemBuilder()
            .WithProductId(Guid.NewGuid())
            .WithProductName("Produto")
            .WithQuantity(1)
            .WithUnitPrice(0);

        var result = builder.Build();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O preço unitário deve ser maior que zero"));
    }
}