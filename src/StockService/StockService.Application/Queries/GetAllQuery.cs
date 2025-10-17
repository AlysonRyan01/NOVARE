using MediatR;
using SharedService.Shared;
using StockService.Domain.Entities;

namespace StockService.Application.Queries;

public record GetAllQuery(int PageNumber, int PageSize) : IRequest<Result<IEnumerable<Product>>>;