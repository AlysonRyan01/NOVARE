using FluentValidation;
using InvoiceService.Application.Mappers.Invoices;
using InvoiceService.Application.Queries.Invoices;
using InvoiceService.Domain.Repositories.Invoices;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Invoices;

public class GetInvoiceByIdHandler : IRequestHandler<GetByIdQuery, Result<InvoiceDto>>
{
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IValidator<GetByIdQuery> _validator;

    public GetInvoiceByIdHandler(IInvoiceQueryRepository invoiceQueryRepository, IValidator<GetByIdQuery> validator)
    {
        _invoiceQueryRepository = invoiceQueryRepository;
        _validator = validator;
    }

    public async Task<Result<InvoiceDto>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return Result<InvoiceDto>.Fail(validationResult.Errors!);

        var invoiceResult = await LoadInvoiceAsync(request.Id);
        if (!invoiceResult.IsSuccess)
            return Result<InvoiceDto>.Fail(invoiceResult.Errors!);

        return Result<InvoiceDto>.Ok(invoiceResult.Value!.ToDto());
    }

    private async Task<Result> ValidateRequestAsync(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<Domain.AggregateRoots.Invoice>> LoadInvoiceAsync(Guid id)
    {
        var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(id);
        if (!invoiceResult.IsSuccess || invoiceResult.Value == null)
            return Result<Domain.AggregateRoots.Invoice>.Fail(invoiceResult.Errors!);

        return Result<Domain.AggregateRoots.Invoice>.Ok(invoiceResult.Value);
    }
}
