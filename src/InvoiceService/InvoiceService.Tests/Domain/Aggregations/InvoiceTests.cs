using InvoiceService.Domain.Builders;
using InvoiceService.Domain.Entities;

namespace InvoiceService.Tests.Domain.Aggregations;

[TestClass]
public class InvoiceTests
{
    [TestMethod]
    public void Build_ShouldReturnSuccess_WhenAllPropertiesAreValid()
    {
        var customerId = Guid.NewGuid();
        var item1 = new InvoiceItemBuilder()
            .WithProductId(Guid.NewGuid())
            .WithProductName("Produto 1")
            .WithQuantity(2)
            .WithUnitPrice(10)
            .Build().Value!;

        var item2 = new InvoiceItemBuilder()
            .WithProductId(Guid.NewGuid())
            .WithProductName("Produto 2")
            .WithQuantity(1)
            .WithUnitPrice(20)
            .Build().Value!;

        var items = new List<InvoiceItem> { item1, item2 };

        var builder = new InvoiceBuilder()
            .WithNumber("INV-001")
            .WithCustomerId(customerId)
            .WithItems(items);

        var result = builder.Build();

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("INV-001", result.Value!.Number.Value);
        Assert.AreEqual(customerId, result.Value.CustomerId);
        Assert.HasCount(items.Count, result.Value.Items);
    }

    [TestMethod]
    public void Build_ShouldReturnFail_WhenNumberIsEmpty()
    {
        var customerId = Guid.NewGuid();
        var item = new InvoiceItemBuilder()
            .WithProductId(Guid.NewGuid())
            .WithProductName("Produto 1")
            .WithQuantity(2)
            .WithUnitPrice(10)
            .Build().Value!;

        var builder = new InvoiceBuilder()
            .WithNumber("")
            .WithCustomerId(customerId)
            .WithItems(new List<InvoiceItem> { item });

        var result = builder.Build();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O número da nota fiscal é obrigatório"));
    }

    [TestMethod]
    public void Build_ShouldReturnFail_WhenCustomerIdIsEmpty()
    {
        var item = new InvoiceItemBuilder()
            .WithProductId(Guid.NewGuid())
            .WithProductName("Produto 1")
            .WithQuantity(2)
            .WithUnitPrice(10)
            .Build().Value!;

        var builder = new InvoiceBuilder()
            .WithNumber("INV-002")
            .WithCustomerId(Guid.Empty)
            .WithItems(new List<InvoiceItem> { item });

        var result = builder.Build();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O ID do cliente é obrigatório"));
    }

    [TestMethod]
    public void Build_ShouldReturnFail_WhenItemsAreEmpty()
    {
        var customerId = Guid.NewGuid();
        var builder = new InvoiceBuilder()
            .WithNumber("INV-003")
            .WithCustomerId(customerId)
            .WithItems(new List<InvoiceItem>());

        var result = builder.Build();

        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("A nota fiscal precisa ter pelo menos um produto"));
    }
}
