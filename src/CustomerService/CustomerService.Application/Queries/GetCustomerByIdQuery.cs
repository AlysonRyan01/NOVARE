using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace CustomerService.Application.Queries;

public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;