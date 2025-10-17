using FluentValidation;
using InvoiceService.Application.Mappers;
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

    public GetInvoiceByIdHandler(IInvoiceQueryRepository invoiceQueryRepository,  IValidator<GetByIdQuery> validator)
    {
        _invoiceQueryRepository = invoiceQueryRepository;
        _validator = validator;
    }

    public async Task<Result<InvoiceDto>> Handle(
        GetByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<InvoiceDto>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(request.Id);
        if (!invoiceResult.IsSuccess || invoiceResult.Value == null)
            return Result<InvoiceDto>.Fail(invoiceResult.Errors!);
        
        var invoice = invoiceResult.Value;

        var invoiceDto = invoice.ToDto();

        return Result<InvoiceDto>.Ok(invoiceDto);
    }
}