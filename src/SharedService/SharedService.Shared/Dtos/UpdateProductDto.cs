namespace SharedService.Shared.Dtos;

public record UpdateProductDto(
    Guid Id,
    string Name, 
    string Description, 
    decimal Price);