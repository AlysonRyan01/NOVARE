using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Contracts;
using InvoiceService.Domain.Entities;
using InvoiceService.Domain.ValueObjects.Invoices;
using SharedService.Shared;

namespace InvoiceService.Domain.Builders;

public class InvoiceBuilder : IBuilder<Invoice>
{
    private string _number = null!;
    private Guid _customerId;
    private List<InvoiceItem> _items = null!;

    public InvoiceBuilder WithNumber(string number)
    {
        _number = number;
        return this;
    }

    public InvoiceBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public InvoiceBuilder WithItems(List<InvoiceItem> items)
    {
        _items = items;
        return this;
    }
    
    public Result<Invoice> Build()
    {
        if (string.IsNullOrEmpty(_number))
            return Result<Invoice>.Fail(["O número da nota fiscal é obrigatório"]);
        
        if (_customerId.Equals(Guid.Empty))
            return Result<Invoice>.Fail(["O ID do cliente é obrigatório"]);
        
        if (!_items.Any())
            return Result<Invoice>.Fail(["A nota fiscal precisa ter pelo menos um produto"]);

        var numberResult = Number.Create(_number);
        if (!numberResult.IsSuccess || numberResult.Value == null)
            return Result<Invoice>.Fail(numberResult.Errors!);
        
        var invoice = new Invoice(numberResult.Value, _customerId, _items);
        
        return Result<Invoice>.Ok(invoice);
    }
}