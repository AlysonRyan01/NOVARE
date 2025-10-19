using CustomerService.Application.Mappers;
using CustomerService.Application.Queries;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace CustomerService.Application.Handlers;

public class GetAllCustomersHandler : IRequestHandler<GetAllCustomersQuery, Result<IEnumerable<CustomerDto>>>
{
    private readonly ICustomerQueryRepository _customerQueryRepository;

    public GetAllCustomersHandler(ICustomerQueryRepository customerQueryRepository)
    {
        _customerQueryRepository = customerQueryRepository;
    }

    public async Task<Result<IEnumerable<CustomerDto>>> Handle(
        GetAllCustomersQuery request, 
        CancellationToken cancellationToken)
    {
        var customersResult = await GetCustomersAsync();
        if (!customersResult.IsSuccess) 
            return Result<IEnumerable<CustomerDto>>.Fail(customersResult.Errors!);

        var customersDto = MapToDto(customersResult.Value!);
        return Result<IEnumerable<CustomerDto>>.Ok(customersDto);
    }

    private async Task<Result<IEnumerable<Customer>>> GetCustomersAsync()
    {
        var result = await _customerQueryRepository.GetAllAsync();
        if (!result.IsSuccess || result.Value == null)
            return Result<IEnumerable<Customer>>.Fail(result.Errors!);

        return Result<IEnumerable<Customer>>.Ok(result.Value);
    }

    private IEnumerable<CustomerDto> MapToDto(IEnumerable<Customer> customers)
    {
        return customers.ToDto();
    }
}