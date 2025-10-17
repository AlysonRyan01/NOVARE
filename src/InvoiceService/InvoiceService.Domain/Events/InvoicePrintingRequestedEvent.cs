using InvoiceService.Domain.Abstractions;
using InvoiceService.Domain.Entities;

namespace InvoiceService.Domain.Events;

public record InvoicePrintingRequestedEvent(Guid InvoiceId, List<InvoiceItem> Items) : DomainEvent;
