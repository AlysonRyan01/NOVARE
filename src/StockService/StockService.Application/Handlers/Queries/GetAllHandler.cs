using FluentValidation;
using MediatR;
using SharedService.Shared;
using StockService.Application.Queries;
using StockService.Domain.Entities;
using StockService.Domain.Repositories;

namespace StockService.Application.Handlers.Queries;

public class GetAllHandler : IRequestHandler<GetAllQuery, Result<IEnumerable<Product>>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IValidator<GetAllQuery> _validator;

    public GetAllHandler(
        IProductQueryRepository productQueryRepository,
        IValidator<GetAllQuery> validator)
    {
        _productQueryRepository = productQueryRepository;
        _validator = validator;
    }

    public async Task<Result<IEnumerable<Product>>> Handle(GetAllQuery request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return Result<IEnumerable<Product>>.Fail(validationResult.Errors!);

        return await GetProductsAsync(request, cancellationToken);
    }

    private async Task<Result> ValidateRequestAsync(GetAllQuery request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
            return Result.Fail(result.Errors.Select(x => x.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<IEnumerable<Product>>> GetProductsAsync(GetAllQuery request, CancellationToken cancellationToken)
    {
        var result = await _productQueryRepository.GetAllAsync(request.PageNumber, request.PageSize, cancellationToken);

        if (!result.IsSuccess)
            return Result<IEnumerable<Product>>.Fail(result.Errors ?? ["Erro ao buscar produtos"]);

        return Result<IEnumerable<Product>>.Ok(result.Value ?? Enumerable.Empty<Product>());
    }
}