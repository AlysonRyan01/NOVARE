namespace SharedService.Shared.Dtos;

public record InvoiceItemRequest(Guid ProductId, int Quantity);