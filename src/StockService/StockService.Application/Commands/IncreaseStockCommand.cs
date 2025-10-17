using MediatR;
using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Application.Commands;

public record IncreaseStockCommand(Guid ProductId, int Quantity) : IRequest<Result<Product>>;
