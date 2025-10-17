using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Queries.Customers;

public record GetAllCustomersQuery() : IRequest<Result<IEnumerable<CustomerDto>>>;