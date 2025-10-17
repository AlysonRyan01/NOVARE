using InvoiceService.Domain.Contracts;
using InvoiceService.Domain.Entities;
using SharedService.Shared;

namespace InvoiceService.Domain.Builders;

public class InvoiceItemBuilder : IBuilder<InvoiceItem>
{
    private Guid _productId = Guid.Empty;
    private string? _productName;
    private int _quantity;
    private decimal _unitPrice;

    public InvoiceItemBuilder WithProductId(Guid productId)
    {
        _productId = productId;
        return this;
    }

    public InvoiceItemBuilder WithProductName(string productName)
    {
        _productName = productName;
        return this;
    }

    public InvoiceItemBuilder WithQuantity(int quantity)
    {
        _quantity = quantity;
        return this;
    }

    public InvoiceItemBuilder WithUnitPrice(decimal unitPrice)
    {
        _unitPrice = unitPrice;
        return this;
    }

    public Result<InvoiceItem> Build()
    {
        if (_productId == Guid.Empty)
            return Result<InvoiceItem>.Fail(["O ID do produto é obrigatório"]);

        if (string.IsNullOrWhiteSpace(_productName))
            return Result<InvoiceItem>.Fail(["O nome do produto é obrigatório"]);

        if (_quantity <= 0)
            return Result<InvoiceItem>.Fail(["A quantidade deve ser maior que zero"]);

        if (_unitPrice <= 0)
            return Result<InvoiceItem>.Fail(["O preço unitário deve ser maior que zero"]);

        var item = new InvoiceItem(_productId, _productName!, _quantity, _unitPrice);
        return Result<InvoiceItem>.Ok(item);
    }
}