using FluentValidation;
using StockService.Application.Commands;

namespace Stock.Application.Validations.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do produto é obrigatório")
            .NotEqual(Guid.Empty).WithMessage("O identificador do produto é inválido");
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do produto é obrigatório")
            .MinimumLength(2).WithMessage("O nome deve ter pelo menos 2 caracteres")
            .MaximumLength(200).WithMessage("O nome não pode exceder 200 caracteres")
            .WithMessage("O nome contém caracteres inválidos");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição do produto é obrigatória")
            .MinimumLength(10).WithMessage("A descrição deve ter pelo menos 10 caracteres")
            .MaximumLength(1000).WithMessage("A descrição não pode exceder 1000 caracteres")
            .WithMessage("A descrição contém caracteres inválidos");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero")
            .LessThanOrEqualTo(999999.99m).WithMessage("O preço não pode exceder 999.999,99")
            .PrecisionScale(8, 2, false).WithMessage("O preço deve ter no máximo 2 casas decimais");
    }
}