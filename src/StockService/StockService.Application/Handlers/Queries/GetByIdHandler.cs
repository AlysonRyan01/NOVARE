using FluentValidation;
using MediatR;
using StockService.Application.Queries;
using SharedService.Shared;
using StockService.Domain.Entities;
using StockService.Domain.Repositories;

namespace StockService.Application.Handlers.Queries;

public class GetByIdHandler : IRequestHandler<GetByIdQuery, Result<Product>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IValidator<GetByIdQuery> _validator;

    public GetByIdHandler(
        IProductQueryRepository productQueryRepository, 
        IValidator<GetByIdQuery> validator)
    {
        _productQueryRepository = productQueryRepository;
        _validator = validator;
    }

    public async Task<Result<Product>> Handle(GetByIdQuery request, CancellationToken cancellationToken = default)
    {
        var validationResult = ValidateRequest(request);
        if (!validationResult.IsSuccess)
            return Result<Product>.Fail(validationResult.Errors!);

        return await GetProductByIdAsync(request.Id, cancellationToken);
    }

    private Result ValidateRequest(GetByIdQuery request)
    {
        var result = _validator.Validate(request);
        if (!result.IsValid)
            return Result.Fail(result.Errors.Select(x => x.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<Product>> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _productQueryRepository.GetByIdAsync(productId, cancellationToken);

        if (!result.IsSuccess)
            return Result<Product>.Fail(result.Errors ?? ["Erro ao buscar produto"]);

        if (result.Value == null)
            return Result<Product>.Fail(["Produto não encontrado"]);

        return Result<Product>.Ok(result.Value);
    }
}