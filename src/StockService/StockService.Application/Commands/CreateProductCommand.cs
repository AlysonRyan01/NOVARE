using MediatR;
using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Application.Commands;

public record CreateProductCommand(
    string Name, 
    string Description, 
    decimal Price, 
    int Quantity) : IRequest<Result<Product>>;