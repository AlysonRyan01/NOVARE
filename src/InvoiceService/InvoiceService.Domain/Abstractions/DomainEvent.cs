using InvoiceService.Domain.Contracts;

namespace InvoiceService.Domain.Abstractions;

public record DomainEvent :IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}