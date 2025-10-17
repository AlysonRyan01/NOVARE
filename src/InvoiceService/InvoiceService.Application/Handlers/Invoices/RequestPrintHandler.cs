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

    public async Task<Result<InvoiceDto>> Handle(
        RequestPrintCommand request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<InvoiceDto>.Fail(validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        
        var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(request.InvoiceId);
        if  (!invoiceResult.IsSuccess || invoiceResult.Value == null)
            return Result<InvoiceDto>.Fail(invoiceResult.Errors!);
        
        var invoice = invoiceResult.Value;
        
        invoice.RebuildState();
        
        var printResult = invoice.RequestPrint();
        if (!printResult.IsSuccess)
            return Result<InvoiceDto>.Fail(printResult.Errors!);

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            await _invoiceCommandRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _domainEventDispatcher.DispatchEventsAsync(cancellationToken);
            
            var invoiceDto = invoice.ToDto();
            
            return Result<InvoiceDto>.Ok(invoiceDto);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<InvoiceDto>.Fail(["Erro ao solicitar impressão"]);
        }
    }
}