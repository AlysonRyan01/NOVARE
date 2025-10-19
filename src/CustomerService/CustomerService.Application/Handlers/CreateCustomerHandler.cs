using CustomerService.Application.Commands;
using CustomerService.Application.Mappers;
using CustomerService.Application.Services;
using CustomerService.Domain.Entities;
using CustomerService.Domain.Repositories;
using FluentValidation;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace CustomerService.Application.Handlers;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerCommandRepository _customerCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCustomerCommand> _validator;

    public CreateCustomerHandler(
        ICustomerCommandRepository customerCommandRepository, 
        IUnitOfWork unitOfWork,
        IValidator<CreateCustomerCommand> validator)
    {
        _customerCommandRepository = customerCommandRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<CustomerDto>> Handle(
        CreateCustomerCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validateRequestResult = await ValidateRequest(request, cancellationToken);
        if (!validateRequestResult.IsSuccess)
            return Result<CustomerDto>.Fail(validateRequestResult.Errors!);
        
        var customerBuilderResult = Customer.Create(request.Name, request.Email, request.Phone, request.Document);
        if (!customerBuilderResult.IsSuccess || customerBuilderResult.Value == null)
            return Result<CustomerDto>.Fail(customerBuilderResult.Errors!);
        
        var customer = customerBuilderResult.Value;

        return await SaveCustomerAsync(customer, cancellationToken);
    }

    private async Task<Result<bool>> ValidateRequest(
        CreateCustomerCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<bool>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        return Result<bool>.Ok(true);
    }

    private async Task<Result<CustomerDto>> SaveCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            await _customerCommandRepository.AddAsync(customer);
            await _unitOfWork.CommitAsync(cancellationToken);

            var customerDto = customer.ToDto();

            return Result<CustomerDto>.Ok(customerDto);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<CustomerDto>.Fail(["E-mail ou CPF já cadastrados"]);
        }
    }
}