using MediatR;
using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Application.Commands;

public record DeleteProductCommand(Guid Id) :  IRequest<Result<Guid>>;