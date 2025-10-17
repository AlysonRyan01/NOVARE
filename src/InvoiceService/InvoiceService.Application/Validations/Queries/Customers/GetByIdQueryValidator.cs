using FluentValidation;
using InvoiceService.Application.Queries.Customers;

namespace InvoiceService.Application.Validations.Queries.Customers;

public class GetByIdQueryValidator :  AbstractValidator<GetByIdQuery>
{
    public GetByIdQueryValidator()
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