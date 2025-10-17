using InvoiceService.Domain.Abstractions;

namespace InvoiceService.Domain.Events;

public record InvoicePrintedEvent(Guid InvoiceId) : DomainEvent;