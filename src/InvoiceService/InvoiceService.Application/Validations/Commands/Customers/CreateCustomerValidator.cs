using FluentValidation;
using InvoiceService.Application.Commands.Customers;

namespace InvoiceService.Application.Validations.Commands.Customers;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(2).WithMessage("Nome deve ter pelo menos 2 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MinimumLength(10).WithMessage("Telefone deve ter pelo menos 10 dígitos");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Documento é obrigatório")
            .MinimumLength(11).WithMessage("Documento deve ter pelo menos 11 dígitos");
    }
}