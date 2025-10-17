using MediatR;
using SharedService.Shared;

namespace InvoiceService.Application.Commands.Invoices;

public record DeleteInvoiceCommand(Guid InvoiceId) :  IRequest<Result<Guid>>;