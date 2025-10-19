using StockService.Domain.Builders;
using StockService.Domain.Entities;
using StockService.Domain.ValueObjects.Product;
[assembly: DoNotParallelize]
namespace StockService.Tests.Domain.Entities;

[TestClass]
public class ProductTests
{
    private ProductBuilder _builder = null!;
    private Name _validName = null!;
    private Description _validDescription = null!;
    private Price _validPrice = null!;
    private StockQuantity _validStockQuantity = null!;

    [TestInitialize]
    public void Initialize()
    {
        _builder = new ProductBuilder();
        _validName = Name.Create("Notebook Dell").Value!;
        _validDescription = Description.Create("Notebook i7, 16GB RAM, 512GB SSD").Value!;
        _validPrice = Price.Create(2500.00m).Value!;
        _validStockQuantity = StockQuantity.Create(50).Value!;
    }

    [TestMethod]
    public void Build_WithValidParameters_ShouldCreateProductSuccessfully()
    {
        var result = _builder
            .WithName(_validName)
            .WithDescription(_validDescription)
            .WithPrice(_validPrice)
            .WithStockQuantity(_validStockQuantity)
            .Build();
        
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(_validName.Value, result.Value.Name.Value);
        Assert.AreEqual(_validDescription.Value, result.Value.Description.Value);
        Assert.AreEqual(_validPrice.Value, result.Value.Price.Value);
        Assert.AreEqual(_validStockQuantity.Value, result.Value.StockQuantity.Value);
        Assert.AreNotEqual(Guid.Empty, result.Value.Id);
        Assert.IsTrue(result.Value.CreatedAt <= DateTime.UtcNow);
        Assert.IsTrue(result.Value.UpdatedAt <= DateTime.UtcNow);
    }

    [TestMethod]
    public void Build_WithoutName_ShouldReturnFailure()
    {
        var result = _builder
            .WithDescription(_validDescription)
            .WithPrice(_validPrice)
            .WithStockQuantity(_validStockQuantity)
            .Build();
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O nome do produto é obrigatório"));
    }

    [TestMethod]
    public void Build_WithoutDescription_ShouldReturnFailure()
    {
        var result = _builder
            .WithName(_validName)
            .WithPrice(_validPrice)
            .WithStockQuantity(_validStockQuantity)
            .Build();
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("A descrição é obrigatória"));
    }

    [TestMethod]
    public void Build_WithoutPrice_ShouldReturnFailure()
    {
        var result = _builder
            .WithName(_validName)
            .WithDescription(_validDescription)
            .WithStockQuantity(_validStockQuantity)
            .Build();
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O preço é obrigatório"));
    }

    [TestMethod]
    public void Build_WithoutStockQuantity_ShouldReturnFailure()
    {
        var result = _builder
            .WithName(_validName)
            .WithDescription(_validDescription)
            .WithPrice(_validPrice)
            .Build();
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("O estoque é obrigatório"));
    }

    [TestMethod]
    public void HasSufficientStock_WithSufficientStock_ShouldReturnTrue()
    {
        var product = CreateValidProduct();
        var requestedQuantity = 25;
        
        var hasSufficientStock = product.HasSufficientStock(requestedQuantity);
        
        Assert.IsTrue(hasSufficientStock);
    }

    [TestMethod]
    public void HasSufficientStock_WithInsufficientStock_ShouldReturnFalse()
    {
        var product = CreateValidProduct();
        var requestedQuantity = 60;
        
        var hasSufficientStock = product.HasSufficientStock(requestedQuantity);
        
        Assert.IsFalse(hasSufficientStock);
    }

    [TestMethod]
    public void HasSufficientStock_WithExactStock_ShouldReturnTrue()
    {
        var product = CreateValidProduct();
        var requestedQuantity = 50; // Exact stock quantity
        
        var hasSufficientStock = product.HasSufficientStock(requestedQuantity);
        
        Assert.IsTrue(hasSufficientStock);
    }

    [TestMethod]
    public void DecreaseStock_WithValidQuantity_ShouldDecreaseStockSuccessfully()
    {
        var product = CreateValidProduct();
        var initialStock = product.StockQuantity.Value;
        var quantityToDecrease = 10;
        
        var result = product.DecreaseStock(quantityToDecrease);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(initialStock - quantityToDecrease, result.Value!.StockQuantity.Value);
        Assert.IsTrue(result.Value.UpdatedAt >= product.UpdatedAt);
    }

    [TestMethod]
    public void DecreaseStock_WithInsufficientStock_ShouldReturnFailure()
    {
        var product = CreateValidProduct();
        var quantityToDecrease = 60; 
        
        var result = product.DecreaseStock(quantityToDecrease);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("Estoque insuficiente"));
        Assert.AreEqual(50, product.StockQuantity.Value);
    }

    [TestMethod]
    public void DecreaseStock_WithNegativeQuantity_ShouldReturnFailure()
    {
        var product = CreateValidProduct();
        var quantityToDecrease = -5;
        
        var result = product.DecreaseStock(quantityToDecrease);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("A quantidade deve ser maior que zero"));
    }

    [TestMethod]
    public void DecreaseStock_WithZeroQuantity_ShouldReturnFailure()
    {
        var product = CreateValidProduct();
        var quantityToDecrease = 0;
        
        var result = product.DecreaseStock(quantityToDecrease);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("A quantidade deve ser maior que zero"));
    }

    [TestMethod]
    public void IncreaseStock_WithValidQuantity_ShouldIncreaseStockSuccessfully()
    {
        var product = CreateValidProduct();
        var initialStock = product.StockQuantity.Value;
        var quantityToIncrease = 10;
        
        var result = product.IncreaseStock(quantityToIncrease);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(initialStock + quantityToIncrease, result.Value!.StockQuantity.Value);
        Assert.IsTrue(result.Value.UpdatedAt >= product.UpdatedAt);
    }

    [TestMethod]
    public void IncreaseStock_WithNegativeQuantity_ShouldReturnFailure()
    {
        var product = CreateValidProduct();
        var quantityToIncrease = -5;
        
        var result = product.IncreaseStock(quantityToIncrease);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("A quantidade deve ser maior que zero"));
    }

    [TestMethod]
    public void IncreaseStock_WithZeroQuantity_ShouldReturnFailure()
    {
        var product = CreateValidProduct();
        var quantityToIncrease = 0;
        
        var result = product.IncreaseStock(quantityToIncrease);
        
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.Errors!.Contains("A quantidade deve ser maior que zero"));
    }

    [TestMethod]
    public void UpdateBasicInfo_WithValidParameters_ShouldUpdateSuccessfully()
    {
        var product = CreateValidProduct();
        var newName = Name.Create("Notebook HP Updated").Value!;
        var newDescription = Description.Create("Notebook atualizado i9, 32GB RAM").Value!;
        var newPrice = Price.Create(3000.00m).Value!;
        
        var result = product.UpdateBasicInfo(newName, newDescription, newPrice);
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(newName.Value, result.Value!.Name.Value);
        Assert.AreEqual(newDescription.Value, result.Value.Description.Value);
        Assert.AreEqual(newPrice.Value, result.Value.Price.Value);
        Assert.IsTrue(result.Value.UpdatedAt >= product.UpdatedAt);
    }

    [TestMethod]
    public void MultipleStockOperations_ShouldMaintainConsistency()
    {
        var product = CreateValidProduct();
        
        var increaseResult = product.IncreaseStock(20);
        Assert.IsTrue(increaseResult.IsSuccess);
        Assert.AreEqual(70, increaseResult.Value!.StockQuantity.Value);

        var decreaseResult = increaseResult.Value.DecreaseStock(30);
        Assert.IsTrue(decreaseResult.IsSuccess);
        Assert.AreEqual(40, decreaseResult.Value!.StockQuantity.Value);

        var finalDecreaseResult = decreaseResult.Value.DecreaseStock(40);
        Assert.IsTrue(finalDecreaseResult.IsSuccess);
        Assert.AreEqual(0, finalDecreaseResult.Value!.StockQuantity.Value);
    }

    [TestMethod]
    public void DecreaseStock_ToZero_ShouldBeSuccessful()
    {
        var product = CreateValidProduct();
        
        var result = product.DecreaseStock(50); // Decrease all stock
        
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(0, result.Value!.StockQuantity.Value);
    }

    private Product CreateValidProduct()
    {
        return _builder
            .WithName(_validName)
            .WithDescription(_validDescription)
            .WithPrice(_validPrice)
            .WithStockQuantity(_validStockQuantity)
            .Build()
            .Value!;
    }
}