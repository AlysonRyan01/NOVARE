using InvoiceService.Application.Commands.Customers;
using InvoiceService.Application.Mappers.Customers;
using InvoiceService.Application.Services;
using InvoiceService.Domain.Builders;
using InvoiceService.Domain.Repositories.Customers;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Customers;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerCommandRepository _customerCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(
        ICustomerCommandRepository customerCommandRepository, 
        IUnitOfWork unitOfWork)
    {
        _customerCommandRepository = customerCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerDto>> Handle(
        CreateCustomerCommand request, 
        CancellationToken cancellationToken = default)
    {
        var nameResult = Domain.ValueObjects.Customers.Name.Create(request.Name);
        if (!nameResult.IsSuccess)
            return Result<CustomerDto>.Fail(nameResult.Errors!);

        var emailResult = Domain.ValueObjects.Customers.Email.Create(request.Email);
        if (!emailResult.IsSuccess)
            return Result<CustomerDto>.Fail(emailResult.Errors!);

        var phoneResult = Domain.ValueObjects.Customers.Phone.Create(request.Phone);
        if (!phoneResult.IsSuccess)
            return Result<CustomerDto>.Fail(phoneResult.Errors!);

        var documentResult = Domain.ValueObjects.Customers.Document.Create(request.Document);
        if (!documentResult.IsSuccess)
            return Result<CustomerDto>.Fail(documentResult.Errors!);
        
        var customerBuilder = new CustomerBuilder()
            .WithName(nameResult.Value!.Value)
            .WithEmail(emailResult.Value!.Value)
            .WithPhone(phoneResult.Value!.Value)
            .WithDocument(documentResult.Value!.Value)
            .Build();

        if (!customerBuilder.IsSuccess || customerBuilder.Value == null)
            return Result<CustomerDto>.Fail(customerBuilder.Errors!);
        
        var customer = customerBuilder.Value;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            await _customerCommandRepository.AddAsync(customer);
            await _unitOfWork.CommitAsync(cancellationToken);

            var customerDto = customer.ToDto();

            return Result<CustomerDto>.Ok(customerDto);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<CustomerDto>.Fail(["Erro ao criar o cliente: " + ex.Message]);
        }
    }
}