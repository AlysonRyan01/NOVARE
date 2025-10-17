namespace SharedService.Shared.Dtos;

public record CreateProductDto(
    string Name, 
    string Description, 
    decimal Price, 
    int Quantity);