using FluentValidation;
using StockService.Application.Commands;

namespace Stock.Application.Validations.Commands;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do produto é obrigatório")
            .NotEqual(Guid.Empty).WithMessage("O identificador do produto é inválido");
    }
}