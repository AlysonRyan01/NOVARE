using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Queries.Invoices;

public record GetAllQuery(int PageNumber, int PageSize) : IRequest<Result<IEnumerable<InvoiceDto>>>;