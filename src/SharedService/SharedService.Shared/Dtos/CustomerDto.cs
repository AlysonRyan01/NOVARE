namespace SharedService.Shared.Dtos;

public record CustomerDto(
    Guid Id,
    string Name,
    string Email,
    string Phone,
    string Document);