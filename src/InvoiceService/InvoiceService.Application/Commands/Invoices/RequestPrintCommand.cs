using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Commands.Invoices;

public record RequestPrintCommand(Guid InvoiceId) : IRequest<Result<InvoiceDto>>;