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
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return Result<InvoiceDto>.Fail(validationResult.Errors!);

        var invoiceResult = await LoadInvoiceAsync(request.InvoiceId);
        if (!invoiceResult.IsSuccess)
            return Result<InvoiceDto>.Fail(invoiceResult.Errors!);

        var invoice = invoiceResult.Value!;

        var updatedItemsResult = BuildInvoiceItems(request);
        if (!updatedItemsResult.IsSuccess)
            return Result<InvoiceDto>.Fail(updatedItemsResult.Errors!);

        var updateResult = invoice.Update(request.CustomerId, updatedItemsResult.Value!);
        if (!updateResult.IsSuccess)
            return Result<InvoiceDto>.Fail(updateResult.Errors!);

        return await PersistInvoiceAsync(invoice, cancellationToken);
    }

    private async Task<Result> ValidateRequestAsync(
        UpdateInvoiceCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<Domain.AggregateRoots.Invoice>> LoadInvoiceAsync(Guid invoiceId)
    {
        var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(invoiceId);
        if (!invoiceResult.IsSuccess || invoiceResult.Value == null)
            return Result<Domain.AggregateRoots.Invoice>.Fail(invoiceResult.Errors!);

        return Result<Domain.AggregateRoots.Invoice>.Ok(invoiceResult.Value);
    }

    private Result<List<Domain.Entities.InvoiceItem>> BuildInvoiceItems(UpdateInvoiceCommand request)
    {
        var items = new List<Domain.Entities.InvoiceItem>();

        foreach (var itemCommand in request.Items)
        {
            var itemBuilder = new InvoiceItemBuilder()
                .WithProductId(itemCommand.ProductId)
                .WithProductName(itemCommand.ProductName)
                .WithQuantity(itemCommand.Quantity)
                .WithUnitPrice(itemCommand.UnitPrice)
                .Build();

            if (!itemBuilder.IsSuccess)
                return Result<List<Domain.Entities.InvoiceItem>>.Fail(itemBuilder.Errors!);

            items.Add(itemBuilder.Value!);
        }

        return Result<List<Domain.Entities.InvoiceItem>>.Ok(items);
    }

    private async Task<Result<InvoiceDto>> PersistInvoiceAsync(
        Domain.AggregateRoots.Invoice invoice, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            await _invoiceCommandRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<InvoiceDto>.Ok(invoice.ToDto());
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<InvoiceDto>.Fail(new[] { "Erro ao atualizar a nota fiscal" });
        }
    }
}
