using FluentValidation;
using InvoiceService.Application.Commands.Invoices;
using InvoiceService.Application.Mappers.Invoices;
using InvoiceService.Application.Services;
using InvoiceService.Domain.Repositories.Invoices;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Invoices;

public class RequestPrintHandler : IRequestHandler<RequestPrintCommand, Result<InvoiceDto>>
{
    private readonly IValidator<RequestPrintCommand> _validator;
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDomainEventDispatcher _domainEventDispatcher;

    public RequestPrintHandler(
        IValidator<RequestPrintCommand> validator, 
        IInvoiceQueryRepository invoiceQueryRepository, 
        IInvoiceCommandRepository invoiceCommandRepository, 
        IUnitOfWork unitOfWork,
        IDomainEventDispatcher domainEventDispatcher)
    {
        _validator = validator;
        _invoiceQueryRepository = invoiceQueryRepository;
        _invoiceCommandRepository = invoiceCommandRepository;
        _unitOfWork = unitOfWork;
        _domainEventDispatcher = domainEventDispatcher;
    }

    public async Task<Result<InvoiceDto>> Handle(RequestPrintCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return Result<InvoiceDto>.Fail(validationResult.Errors!);

        var invoiceResult = await LoadInvoiceAsync(request.InvoiceId);
        if (!invoiceResult.IsSuccess)
            return Result<InvoiceDto>.Fail(invoiceResult.Errors!);

        var invoice = invoiceResult.Value!;
        invoice.RebuildState();

        var printResult = invoice.RequestPrint();
        if (!printResult.IsSuccess)
            return Result<InvoiceDto>.Fail(printResult.Errors!);

        return await PersistInvoiceAsync(invoice, cancellationToken);
    }

    private async Task<Result> ValidateRequestAsync(RequestPrintCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<Domain.AggregateRoots.Invoice>> LoadInvoiceAsync(Guid invoiceId)
    {
        var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(invoiceId);
        if (!invoiceResult.IsSuccess || invoiceResult.Value == null)
            return Result<Domain.AggregateRoots.Invoice>.Fail(invoiceResult.Errors!);

        return Result<Domain.AggregateRoots.Invoice>.Ok(invoiceResult.Value);
    }

    private async Task<Result<InvoiceDto>> PersistInvoiceAsync(Domain.AggregateRoots.Invoice invoice, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _invoiceCommandRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _domainEventDispatcher.DispatchEventsAsync(cancellationToken);

            return Result<InvoiceDto>.Ok(invoice.ToDto());
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<InvoiceDto>.Fail(new[] { "Erro ao solicitar impressão" });
        }
    }
}
