using FluentValidation;
using StockService.Application.Commands;

namespace StockService.Application.Validations.Commands;

public class IncreaseStockCommandValidator : AbstractValidator<IncreaseStockCommand>
{
    public IncreaseStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("O identificador do produto é obrigatório")
            .NotEqual(Guid.Empty).WithMessage("O identificador do produto é inválido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero");
    }
}