using SharedService.Shared;

namespace ProductService.Domain.ValueObjects.Product;

public class StockQuantity
{
    public int Value { get; }
    
    private StockQuantity(int value) => Value = value;

    public static Result<StockQuantity> Create(int value)
    {
        if (value < 0)
            return Result<StockQuantity>.Fail(["O estoque não pode ser menos que 0"]);

        var stockQuantity = new StockQuantity(value);
        
        return Result<StockQuantity>.Ok(stockQuantity);
    }

    public Result<StockQuantity> Add(int amount)
    {
        if (amount < 0)
            return Result<StockQuantity>.Fail(["A quantidade adicionada não pode ser negativa"]);

        return Result<StockQuantity>.Ok(new StockQuantity(Value + amount));
    }

    public Result<StockQuantity> Subtract(int amount)
    {
        if (amount < 0)
            return Result<StockQuantity>.Fail(["A quantidade removida não pode ser negativa"]);

        if (Value - amount < 0)
            return Result<StockQuantity>.Fail(["O estoque não pode ficar negativo"]);

        return Result<StockQuantity>.Ok(new StockQuantity(Value - amount));
    }

    public override string ToString() => Value.ToString();
}