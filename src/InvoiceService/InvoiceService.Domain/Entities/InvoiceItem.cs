using InvoiceService.Domain.AggregateRoots;

namespace InvoiceService.Domain.Entities;

public class InvoiceItem
{
    public Guid InvoiceId { get; private set; }
    public Invoice Invoice { get; private set; } = null!;
    
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    
    internal InvoiceItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}