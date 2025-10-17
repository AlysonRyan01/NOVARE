namespace SharedService.Shared.Dtos;

public record DecreaseStockDto(
    Guid ProductId,
    int Quantity);