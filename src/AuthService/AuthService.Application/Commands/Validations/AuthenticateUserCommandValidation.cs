using FluentValidation;

namespace AuthService.Application.Commands.Validations;

public class AuthenticateUserCommandValidation :  AbstractValidator<AuthenticateUserCommand>
{
    public AuthenticateUserCommandValidation()
    {
        RuleFor(command => command.Email)
            .NotEmpty().WithMessage("Informe um E-mail")
            .EmailAddress().WithMessage("Informe um e-mail válido");
        
        RuleFor(c => c.Password)
            .NotEmpty().WithMessage("Informe uma senha")
            .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres")
            .MaximumLength(50).WithMessage("A senha deve ter no máximo 50 caracteres");
    }
}