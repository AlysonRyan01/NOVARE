using ProductService.Domain.ValueObjects.Product;

namespace ProductService.Tests.Domain.ValueObjects.Products;

[TestClass]
public class StockQuantityTests
{
    [TestMethod]
        public void Create_WithPositiveValue_ShouldSucceed()
        {
            var result = StockQuantity.Create(10);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(10, result.Value!.Value);
        }

        [TestMethod]
        public void Create_WithNegativeValue_ShouldFail()
        {
            var result = StockQuantity.Create(-5);

            Assert.IsFalse(result.IsSuccess);
            CollectionAssert.Contains(result.Errors!.ToList(), "O estoque não pode ser menos que 0");
        }

        [TestMethod]
        public void Add_WithPositiveAmount_ShouldIncreaseStock()
        {
            var stock = StockQuantity.Create(5).Value!;
            var result = stock.Add(3);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(8, result.Value!.Value);
        }

        [TestMethod]
        public void Add_WithNegativeAmount_ShouldFail()
        {
            var stock = StockQuantity.Create(5).Value!;
            var result = stock.Add(-2);

            Assert.IsFalse(result.IsSuccess);
            CollectionAssert.Contains(result.Errors!.ToList(), "A quantidade adicionada não pode ser negativa");
        }

        [TestMethod]
        public void Subtract_WithValidAmount_ShouldDecreaseStock()
        {
            var stock = StockQuantity.Create(10).Value!;
            var result = stock.Subtract(4);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(6, result.Value!.Value);
        }

        [TestMethod]
        public void Subtract_WithNegativeAmount_ShouldFail()
        {
            var stock = StockQuantity.Create(10).Value!;
            var result = stock.Subtract(-3);

            Assert.IsFalse(result.IsSuccess);
            CollectionAssert.Contains(result.Errors!.ToList(), "A quantidade removida não pode ser negativa");
        }

        [TestMethod]
        public void Subtract_MoreThanStock_ShouldFail()
        {
            var stock = StockQuantity.Create(5).Value!;
            var result = stock.Subtract(10);

            Assert.IsFalse(result.IsSuccess);
            CollectionAssert.Contains(result.Errors!.ToList(), "O estoque não pode ficar negativo");
        }
}