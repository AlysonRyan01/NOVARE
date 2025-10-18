namespace SharedService.Shared.Dtos;

public record CreateCustomerDto(
    string Name,
    string Email,
    string Phone,
    string Document);