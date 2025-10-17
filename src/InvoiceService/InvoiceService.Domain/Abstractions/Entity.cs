namespace InvoiceService.Domain.Abstractions;

public abstract class Entity
{
    public Guid Id { get; internal set; }
}