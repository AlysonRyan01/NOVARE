using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace CustomerService.Application.Commands;

public record CreateCustomerCommand(
    string Name, 
    string Email, 
    string Phone, 
    string Document) : IRequest<Result<CustomerDto>>;