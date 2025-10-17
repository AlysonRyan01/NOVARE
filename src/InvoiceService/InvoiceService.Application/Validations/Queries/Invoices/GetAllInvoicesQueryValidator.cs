using FluentValidation;
using InvoiceService.Application.Queries.Invoices;

namespace InvoiceService.Application.Validations.Queries.Invoices;

public class GetAllInvoicesQueryValidator : AbstractValidator<GetAllQuery>
{
    public GetAllInvoicesQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber deve ser maior que 0");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize deve ser maior que 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize não pode exceder 100");
    }
}