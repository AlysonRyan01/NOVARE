namespace SharedService.Shared.Events;

public record OutOfStockNotifier(Guid InvoiceId, IEnumerable<string> Errors);