using FluentValidation;
using StockService.Application.Queries;

namespace ProductService.Application.Validations.Queries;

public class GetAllQueryValidator : AbstractValidator<GetAllQuery>
{
    public GetAllQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("O número da página precisa ser maior que 0");
        
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("O tamanho da página precisa ser maior que 0")
            .LessThanOrEqualTo(100).WithMessage("O tamanho da página não pode exceder 100 itens");
    }
}