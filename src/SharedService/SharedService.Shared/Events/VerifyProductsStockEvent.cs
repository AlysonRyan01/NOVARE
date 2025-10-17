using SharedService.Shared.Dtos;

namespace SharedService.Shared.Events;

public record VerifyProductsStockEvent(Guid InvoiceId, IEnumerable<InvoiceItemRequest> InvoiceItems);