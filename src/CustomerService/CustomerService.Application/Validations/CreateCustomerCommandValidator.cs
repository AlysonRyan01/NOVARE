using CustomerService.Application.Commands;
using FluentValidation;

namespace CustomerService.Application.Validations;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(150).WithMessage("O email deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .Matches(@"^\d{10,11}$").WithMessage("Telefone inválido. Deve conter 10 ou 11 dígitos.");

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("O documento é obrigatório.")
            .Matches(@"^(\d{11}|\d{14})$").WithMessage("Documento inválido. Deve conter 11 ou 14 dígitos.");
    }
}