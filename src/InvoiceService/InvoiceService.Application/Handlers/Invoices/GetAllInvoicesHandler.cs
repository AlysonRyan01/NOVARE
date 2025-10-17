using FluentValidation;
using InvoiceService.Application.Mappers.Invoices;
using InvoiceService.Application.Queries.Invoices;
using InvoiceService.Domain.Repositories.Invoices;
using MediatR;
using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace InvoiceService.Application.Handlers.Invoices;

public class GetAllInvoicesHandler : IRequestHandler<GetAllQuery, Result<IEnumerable<InvoiceDto>>>
{
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IValidator<GetAllQuery> _validator;

    public GetAllInvoicesHandler(IInvoiceQueryRepository invoiceQueryRepository, IValidator<GetAllQuery> validator)
    {
        _invoiceQueryRepository = invoiceQueryRepository;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<InvoiceDto>>> Handle(
        GetAllQuery request, 
        CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<IEnumerable<InvoiceDto>>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
        
        var invoicesResult = await _invoiceQueryRepository.GetPagedAsync(request.PageNumber, request.PageSize);
        if (!invoicesResult.IsSuccess || invoicesResult.Value == null)
            return Result<IEnumerable<InvoiceDto>>.Fail(invoicesResult.Errors!);

        var invoices = invoicesResult.Value;
        
        var invoicesDto = invoices.ToDto();
        
        return Result<IEnumerable<InvoiceDto>>.Ok(invoicesDto);
    }
}