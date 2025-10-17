using FluentValidation;
using StockService.Application.Commands;

namespace ProductService.Application.Validations.Commands;

public class DecreaseStockCommandValidator : AbstractValidator<DecreaseStockCommand>
{
    public DecreaseStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("O identificador do produto é obrigatório")
            .NotEqual(Guid.Empty).WithMessage("O identificador do produto é inválido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero");
    }
}