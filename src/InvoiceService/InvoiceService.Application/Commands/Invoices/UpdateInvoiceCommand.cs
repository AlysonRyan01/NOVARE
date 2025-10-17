using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Commands.Invoices;

public record UpdateInvoiceCommand(
    Guid InvoiceId,
    Guid CustomerId,
    List<InvoiceItemDto> Items) : IRequest<Result<InvoiceDto>>;