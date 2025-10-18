namespace SharedService.Shared.Dtos;

public record InvoiceStatusDto(
    Guid Id,
    string Status,
    DateTime? PrintedAt,
    List<string> Errors,
    DateTime Timestamp);