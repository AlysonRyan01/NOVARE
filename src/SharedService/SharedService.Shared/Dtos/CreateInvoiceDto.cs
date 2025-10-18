namespace SharedService.Shared.Dtos;

public record CreateInvoiceDto(
    Guid CustomerId,
    List<InvoiceItemDto> Items);