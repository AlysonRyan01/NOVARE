using InvoiceService.Domain.Abstractions;
using InvoiceService.Domain.Contracts;
using InvoiceService.Domain.Entities;
using InvoiceService.Domain.Enums;
using InvoiceService.Domain.Events;
using InvoiceService.Domain.States.Invoices;
using InvoiceService.Domain.ValueObjects.Invoices;
using SharedService.Shared;

namespace InvoiceService.Domain.AggregateRoots;

public class Invoice : AggregateRoot
{
    public Number Number { get; internal set; } = null!;
    public EInvoiceStatus Status { get; internal set; }
    
    public List<InvoiceItem> Items { get; internal set; } = new();
    
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = null!;
    
    public decimal Total => Items.Sum(x => x.TotalPrice);
    public DateTime CreatedAt { get; internal set; }
    public DateTime? PrintedAt { get; internal set; }
    private IInvoiceState _state = new PendingState();
    
    public List<string> Errors { get; private set; } = new ();
    
    protected Invoice() { }

    internal Invoice(
        Number number, 
        Guid customerId,
        List<InvoiceItem> items)
    {
        Id = Guid.NewGuid();
        Number = number;
        Items = items;
        Status = EInvoiceStatus.Pending;
        CustomerId = customerId;
        CreatedAt = DateTime.Now;
    }

    internal void ChangeState(IInvoiceState newState) => _state = newState;
    
    public Result<Invoice> RequestPrint()
    {
        var result = _state.ValidationRequested(this);
        if (result.IsSuccess)
        {
            Status = EInvoiceStatus.ValidationRequested;
            
            AddDomainEvent(new InvoicePrintingRequestedEvent(Id, Items));
        }
        return result;
    }
    
    public Result<Invoice> MarkAsPrinted()
    {
        var result = _state.Printed(this);
        if (result.IsSuccess)
        {
            Status = EInvoiceStatus.Printed;
            PrintedAt = DateTime.UtcNow;
            
            AddDomainEvent(new InvoicePrintedEvent(Id));
        }
        return result;
    }

    public Result<Invoice> MarkAsOutOfStock(List<string> errors)
    {
        var result = _state.OutOfStock(this);
        if (result.IsSuccess)
        {
            Status = EInvoiceStatus.OutOfStock;
            Errors = errors;
            
            AddDomainEvent(new InvoiceOutOfStockEvent(Id, errors));
        }
        return result;
    }

    public Result<Invoice> Update(Guid customerId, List<InvoiceItem> items)
    {
        var errors = new List<string>();
    
        if (customerId == Guid.Empty)
            errors.Add("A nota fiscal deve ter um cliente válido");
    
        if (items.Any() != true)
            errors.Add("A nota fiscal deve ter pelo menos um produto");
    
        if (errors.Any())
            return Result<Invoice>.Fail(errors);
    
        CustomerId = customerId;
        Items = items;

        return Result<Invoice>.Ok(this);
    }
    
    public void RebuildState()
    {
        _state = Status switch
        {
            EInvoiceStatus.Pending => new PendingState(),
            EInvoiceStatus.ValidationRequested => new ValidationRequestedState(),
            EInvoiceStatus.OutOfStock => new OutOfStockState(),
            EInvoiceStatus.Printed => new PrintedState(),
            _ => new PendingState()
        };
    }
}