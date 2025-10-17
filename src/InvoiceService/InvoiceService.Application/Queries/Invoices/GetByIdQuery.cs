using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Queries.Invoices;

public record GetByIdQuery(Guid Id) :  IRequest<Result<InvoiceDto>>;