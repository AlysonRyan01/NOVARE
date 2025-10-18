namespace SharedService.Shared.Dtos;

public record UpdateInvoiceDto(
    Guid InvoiceId,
    Guid CustomerId,
    List<InvoiceItemDto> Items);