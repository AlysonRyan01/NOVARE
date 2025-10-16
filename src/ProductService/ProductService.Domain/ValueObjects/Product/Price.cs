using SharedService.Shared;

namespace ProductService.Domain.ValueObjects.Product;

public class Price
{
    public decimal Value { get; }

    private Price(decimal value) => Value = value;

    public static Result<Price> Create(decimal value)
    {
        if (value <= 0)
            return Result<Price>.Fail(["O preço deve ser maior que 0"]);

        var price = new Price(value);
        
        return Result<Price>.Ok(price);
    }

    public override string ToString() => Value.ToString("C");
}