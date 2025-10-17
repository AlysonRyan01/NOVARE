using FluentValidation;
using InvoiceService.Application.Commands.Invoices;
using InvoiceService.Application.Mappers.Invoices;
using InvoiceService.Application.Services;
using InvoiceService.Domain.Builders;
using InvoiceService.Domain.Entities;
using InvoiceService.Domain.Repositories.Invoices;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Invoices;

public class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateInvoiceCommand> _validator;

    public CreateInvoiceHandler(
        IInvoiceCommandRepository invoiceCommandRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateInvoiceCommand> validator)
    {
        _invoiceCommandRepository = invoiceCommandRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<InvoiceDto>> Handle(
        CreateInvoiceCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<InvoiceDto>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        var invoiceItems = new List<InvoiceItem>();
        
        foreach (var itemCommand in request.Items)
        {
            var itemBuilder = new InvoiceItemBuilder()
                .WithProductId(itemCommand.ProductId)
                .WithProductName(itemCommand.ProductName)
                .WithQuantity(itemCommand.Quantity)
                .WithUnitPrice(itemCommand.UnitPrice)
                .Build();

            if (!itemBuilder.IsSuccess)
                return Result<InvoiceDto>.Fail(itemBuilder.Errors!);
            
            invoiceItems.Add(itemBuilder.Value!);
        }
        
        var invoiceBuilder = new InvoiceBuilder()
            .WithNumber(Guid.NewGuid().ToString("N")[..8].ToUpper())
            .WithCustomerId(request.CustomerId)
            .WithItems(invoiceItems)
            .Build();

        if (!invoiceBuilder.IsSuccess || invoiceBuilder.Value is null)
            return Result<InvoiceDto>.Fail(invoiceBuilder.Errors!);
        
        var invoice = invoiceBuilder.Value;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            await _invoiceCommandRepository.AddAsync(invoice);
            await _unitOfWork.CommitAsync(cancellationToken);

            var invoiceDto = invoice.ToDto();

            return Result<InvoiceDto>.Ok(invoiceDto);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<InvoiceDto>.Fail(["Erro ao criar a nota fiscal"]);
        }
    }
}