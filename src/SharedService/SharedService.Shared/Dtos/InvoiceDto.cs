namespace SharedService.Shared.Dtos;

public record InvoiceDto(
    Guid Id,
    string Number,
    string Status,
    Guid CustomerId,
    decimal Total,
    DateTime CreatedAt,
    DateTime? PrintedAt,
    List<string> Errors,
    List<InvoiceItemDto> Items);