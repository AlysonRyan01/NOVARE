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

    public async Task<Result<Guid>> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken = default)
    {
        var validateResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validateResult.IsSuccess)
            return Result<Guid>.Fail(validateResult.Errors!);

        var invoiceResult = await LoadInvoiceAsync(request.InvoiceId);
        if (!invoiceResult.IsSuccess)
            return Result<Guid>.Fail(invoiceResult.Errors!);

        return await DeleteInvoiceAsync(invoiceResult.Value!, cancellationToken);
    }

    private async Task<Result> ValidateRequestAsync(DeleteInvoiceCommand request, CancellationToken cancellationToken)
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

    private async Task<Result<Guid>> DeleteInvoiceAsync(
        Domain.AggregateRoots.Invoice invoice, 
        CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _invoiceCommandRepository.DeleteAsync(invoice.Id);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Guid>.Ok(invoice.Id);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Guid>.Fail(new[] { "Erro ao excluir a nota fiscal" });
        }
    }
}
