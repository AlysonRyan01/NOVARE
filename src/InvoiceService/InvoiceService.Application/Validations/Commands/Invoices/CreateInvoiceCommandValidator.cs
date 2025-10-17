using FluentValidation;
using InvoiceService.Application.Commands.Invoices;

namespace InvoiceService.Application.Validations.Commands.Invoices;

public class CreateInvoiceCommandValidator :  AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId é obrigatório");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("A nota fiscal deve ter pelo menos um item")
            .Must(items => items.All(item => item.Quantity > 0))
            .WithMessage("A quantidade deve ser maior que zero");
    }
}