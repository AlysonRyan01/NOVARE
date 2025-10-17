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

    public async Task<Result<Product>> Handle(
        GetByIdQuery request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return Result<Product>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());
            
        var result = await _productQueryRepository.GetByIdAsync(request.Id);
            
        if (!result.IsSuccess)
            return Result<Product>.Fail(result.Errors ?? ["Erro ao buscar produto"]);
            
        if (result.Value == null)
            return Result<Product>.Fail(["Produto não encontrado"]);
            
        return Result<Product>.Ok(result.Value);
    }
}