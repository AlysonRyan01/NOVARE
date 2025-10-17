using MediatR;
using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Application.Queries;

public record GetByIdQuery(Guid Id) : IRequest<Result<Product>>;