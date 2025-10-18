namespace SharedService.Shared.Dtos;

public record InvoiceItemDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);