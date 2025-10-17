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

    public async Task<Result<IEnumerable<Product>>> Handle(
        GetAllQuery request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<IEnumerable<Product>>.Fail(
                validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            
        var result = await _productQueryRepository.GetAllAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);
            
        if (!result.IsSuccess)
            return Result<IEnumerable<Product>>.Fail(result.Errors ?? ["Erro ao buscar produtos"]);
            
        var products = result.Value ?? Enumerable.Empty<Product>();
            
        return Result<IEnumerable<Product>>.Ok(products);
    }
}