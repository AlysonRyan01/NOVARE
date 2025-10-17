using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Commands.Customers;

public record CreateCustomerCommand(
    string Name,
    string Email,
    string Phone,
    string Document) : IRequest<Result<CustomerDto>>;