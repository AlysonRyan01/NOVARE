using FluentValidation;
using InvoiceService.Application.Commands.Invoices;
using InvoiceService.Application.Services;
using InvoiceService.Domain.Repositories.Invoices;
using MediatR;
using SharedService.Shared;

namespace InvoiceService.Application.Handlers.Invoices;

public class DeleteInvoiceHandler : IRequestHandler<DeleteInvoiceCommand, Result<Guid>>
{
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteInvoiceCommand> _validator;

    public DeleteInvoiceHandler(
        IInvoiceCommandRepository invoiceCommandRepository,
        IInvoiceQueryRepository invoiceQueryRepository,
        IUnitOfWork unitOfWork,
        IValidator<DeleteInvoiceCommand> validator)
    {
        _invoiceCommandRepository = invoiceCommandRepository;
        _invoiceQueryRepository = invoiceQueryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> Handle(
        DeleteInvoiceCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<Guid>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(request.InvoiceId);
        if (!invoiceResult.IsSuccess || invoiceResult.Value == null)
            return Result<Guid>.Fail(invoiceResult.Errors!);
        
        var invoice = invoiceResult.Value;

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            await _invoiceCommandRepository.DeleteAsync(invoice.Id);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(request.InvoiceId);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Guid>.Fail(["Erro ao excluir a nota fiscal"]);
        }
    }
}