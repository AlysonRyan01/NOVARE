using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace CustomerService.Application.Queries;

public record GetAllCustomersQuery() : IRequest<Result<IEnumerable<CustomerDto>>>;