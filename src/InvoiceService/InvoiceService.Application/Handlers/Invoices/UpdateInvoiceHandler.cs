using FluentValidation;
using InvoiceService.Application.Commands.Invoices;
using InvoiceService.Application.Mappers.Invoices;
using InvoiceService.Application.Services;
using InvoiceService.Domain.Builders;
using InvoiceService.Domain.Repositories.Invoices;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Invoices;

public class UpdateInvoiceHandler : IRequestHandler<UpdateInvoiceCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateInvoiceCommand> _validator;

    public UpdateInvoiceHandler(
        IInvoiceCommandRepository invoiceCommandRepository,
        IInvoiceQueryRepository invoiceQueryRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateInvoiceCommand> validator)
    {
        _invoiceCommandRepository = invoiceCommandRepository;
        _invoiceQueryRepository = invoiceQueryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<InvoiceDto>> Handle(
        UpdateInvoiceCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<InvoiceDto>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        var existingInvoiceResult = await _invoiceQueryRepository.GetByIdAsync(request.InvoiceId);
        if (!existingInvoiceResult.IsSuccess || existingInvoiceResult.Value == null)
            return Result<InvoiceDto>.Fail(existingInvoiceResult.Errors!);
        
        var existingInvoice = existingInvoiceResult.Value;
        
        var updatedItems = new List<Domain.Entities.InvoiceItem>();
        
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
            
            updatedItems.Add(itemBuilder.Value!);
        }
        
        var updateResult = existingInvoice.Update(request.CustomerId, updatedItems);
        
        if (!updateResult.IsSuccess)
            return Result<InvoiceDto>.Fail(updateResult.Errors!);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            await _invoiceCommandRepository.UpdateAsync(existingInvoice);
            await _unitOfWork.CommitAsync(cancellationToken);

            var invoiceDto = existingInvoice.ToDto();

            return Result<InvoiceDto>.Ok(invoiceDto);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<InvoiceDto>.Fail([$"Erro ao atualizar a nota fiscal"]);
        }
    }
}