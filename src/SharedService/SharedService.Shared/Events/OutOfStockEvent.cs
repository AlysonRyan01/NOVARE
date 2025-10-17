namespace SharedService.Shared.Events;

public record OutOfStockEvent(Guid InvoiceId, IEnumerable<string> Errors);