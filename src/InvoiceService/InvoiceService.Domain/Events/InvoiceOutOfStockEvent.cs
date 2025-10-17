using InvoiceService.Domain.Abstractions;

namespace InvoiceService.Domain.Events;

public record InvoiceOutOfStockEvent(Guid InvoiceId, List<string> Errors) : DomainEvent;