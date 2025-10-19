using FluentValidation;
using InvoiceService.Application.Commands.Invoices;
using InvoiceService.Application.Mappers.Invoices;
using InvoiceService.Application.Services;
using InvoiceService.Domain.AggregateRoots;
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
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return Result<InvoiceDto>.Fail(validationResult.Errors!);
        
        var invoiceItemsResult = CreateInvoiceItems(request);
        if (!invoiceItemsResult.IsSuccess || invoiceItemsResult.Value == null)
            return Result<InvoiceDto>.Fail(invoiceItemsResult.Errors!);
        
        var invoiceItems = invoiceItemsResult.Value;
        
        var createInvoiceResult = CreateInvoice(invoiceItems, request.CustomerId);
        if (!createInvoiceResult.IsSuccess || createInvoiceResult.Value is null)
            return Result<InvoiceDto>.Fail(createInvoiceResult.Errors!);
        
        var invoice = createInvoiceResult.Value;

        return await SaveInvoiceAndReturnDtoAsync(invoice, cancellationToken);
    }

    private async Task<Result> ValidateRequestAsync(
        CreateInvoiceCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        return Result.Ok();
    }

    private Result<List<InvoiceItem>> CreateInvoiceItems(CreateInvoiceCommand request)
    {
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
                return Result<List<InvoiceItem>>.Fail(itemBuilder.Errors!);

            invoiceItems.Add(itemBuilder.Value!);
        }

        return Result<List<InvoiceItem>>.Ok(invoiceItems);
    }

    private Result<Invoice> CreateInvoice(List<InvoiceItem> invoiceItems, Guid customerId)
    {
        return new InvoiceBuilder()
            .WithNumber(Guid.NewGuid().ToString("N")[..8].ToUpper())
            .WithCustomerId(customerId)
            .WithItems(invoiceItems)
            .Build();
    }

    private async Task<Result<InvoiceDto>> SaveInvoiceAndReturnDtoAsync(
        Invoice invoice, 
        CancellationToken cancellationToken = default)
    {
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