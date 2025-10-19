using CustomerService.Application.Mappers;
using CustomerService.Application.Queries;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using FluentValidation;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace CustomerService.Application.Handlers;

public class GetCustomerByIdHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerQueryRepository _customerQueryRepository;
    private readonly IValidator<GetCustomerByIdQuery> _validator;

    public GetCustomerByIdHandler(
        ICustomerQueryRepository customerQueryRepository, 
        IValidator<GetCustomerByIdQuery> validator)
    {
        _customerQueryRepository = customerQueryRepository;
        _validator = validator;
    }

    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request, 
        CancellationToken cancellationToken = default)
    {
        var validation = await ValidateRequestAsync(request, cancellationToken);
        if (!validation.IsSuccess) 
            return Result<CustomerDto>.Fail(validation.Errors!);

        var customerResult = await GetCustomerAsync(request.Id);
        if (!customerResult.IsSuccess) 
            return Result<CustomerDto>.Fail(customerResult.Errors!);

        return MapToDto(customerResult.Value!);
    }

    private async Task<Result<bool>> ValidateRequestAsync(
        GetCustomerByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<bool>.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        return Result<bool>.Ok(true);
    }

    private async Task<Result<Customer>> GetCustomerAsync(Guid id)
    {
        var result = await _customerQueryRepository.GetAsync(id);
        if (!result.IsSuccess || result.Value == null)
            return Result<Customer>.Fail(result.Errors!);

        return Result<Customer>.Ok(result.Value);
    }

    private Result<CustomerDto> MapToDto(Customer customer)
    {
        return Result<CustomerDto>.Ok(customer.ToDto());
    }
}