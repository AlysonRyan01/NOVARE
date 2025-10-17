namespace SharedService.Shared.Dtos;

public record IncreaseStockDto(
    Guid ProductId,
    int Quantity);