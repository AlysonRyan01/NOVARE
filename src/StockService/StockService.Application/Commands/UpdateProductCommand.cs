using MediatR;
using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Application.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name, 
    string Description, 
    decimal Price) : IRequest<Result<Product>>;