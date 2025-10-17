using FluentValidation;
using StockService.Application.Queries;

namespace ProductService.Application.Validations.Queries;

public class GetByIdQueryValidator : AbstractValidator<GetByIdQuery>
{
    public GetByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do produto é obrigatório")
            .NotEqual(Guid.Empty).WithMessage("O identificador do produto é inválido");
    }
}