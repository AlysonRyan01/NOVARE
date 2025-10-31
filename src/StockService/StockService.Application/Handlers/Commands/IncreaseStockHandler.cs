using FluentValidation;
using MediatR;
using SharedService.Shared;
using StockService.Application.Commands;
using StockService.Application.Services;
using StockService.Domain.Entities;
using StockService.Domain.Repositories;

namespace StockService.Application.Handlers.Commands;

public class IncreaseStockHandler : IRequestHandler<IncreaseStockCommand, Result<Product>>
{
    private readonly IProductCommandRepository _productCommandRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<IncreaseStockCommand> _validator;

    public IncreaseStockHandler(
        IProductCommandRepository productCommandRepository, 
        IProductQueryRepository productQueryRepository, 
        IUnitOfWork unitOfWork, 
        IValidator<IncreaseStockCommand> validator)
    {
        _productCommandRepository = productCommandRepository;
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Product>> Handle(IncreaseStockCommand request, CancellationToken cancellationToken = default)
    {
        var validationResult = ValidateRequest(request);
        if (!validationResult.IsSuccess)
            return Result<Product>.Fail(validationResult.Errors!);

        var productResult = await GetProductAsync(request.ProductId, cancellationToken);
        if (!productResult.IsSuccess)
            return Result<Product>.Fail(productResult.Errors!);

        var product = productResult.Value!;
        
        var increaseResult = product.IncreaseStock(request.Quantity);
        if (!increaseResult.IsSuccess)
            return Result<Product>.Fail(increaseResult.Errors!);

        return await UpdateProductAsync(product, cancellationToken);
    }

    private Result ValidateRequest(IncreaseStockCommand request)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<Product>> GetProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        var exists = await _productQueryRepository.GetByIdAsync(productId, cancellationToken);
        if (!exists.IsSuccess || exists.Value == null)
            return Result<Product>.Fail(exists.Errors ?? ["Produto não encontrado"]);

        return Result<Product>.Ok(exists.Value);
    }

    private async Task<Result<Product>> UpdateProductAsync(Product product, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var updateResult = await _productCommandRepository.UpdateAsync(product, cancellationToken);
            if (!updateResult.IsSuccess)
                return Result<Product>.Fail(updateResult.Errors!);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<Product>.Ok(product);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Product>.Fail(["Erro interno no servidor"]);
        }
    }
}
