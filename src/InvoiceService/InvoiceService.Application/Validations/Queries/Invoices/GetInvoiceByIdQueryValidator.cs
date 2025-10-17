using FluentValidation;
using InvoiceService.Application.Queries.Invoices;

namespace InvoiceService.Application.Validations.Queries.Invoices;

public class GetInvoiceByIdQueryValidator : AbstractValidator<GetByIdQuery>
{
    public GetInvoiceByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID é obrigatório")
            .Must(BeValidGuid).WithMessage("ID deve ser um GUID válido");
    }
    
    private bool BeValidGuid(Guid id)
    {
        return id != Guid.Empty;
    }
}