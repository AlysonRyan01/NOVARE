using FluentValidation;
using InvoiceService.Application.Commands.Invoices;

namespace InvoiceService.Application.Validations.Commands.Invoices;

public class RequestPrintValidator : AbstractValidator<RequestPrintCommand>
{
    public RequestPrintValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty().WithMessage("ID é obrigatório")
            .Must(BeValidGuid).WithMessage("ID deve ser um GUID válido");
    }
    
    private bool BeValidGuid(Guid id)
    {
        return id != Guid.Empty;
    }
}