using CustomerService.Application.Queries;
using FluentValidation;

namespace CustomerService.Application.Validations;

public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
{
    public GetCustomerByIdQueryValidator()
    {
        RuleFor(q => q.Id)
            .NotEmpty()
            .WithMessage("O ID do cliente é obrigatório");
    }
}