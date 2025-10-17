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

    public async Task<Result<Product>> Handle(
        IncreaseStockCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = _validator.Validate(request);
        if  (!validationResult.IsValid)
            return Result<Product>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var exists = await _productQueryRepository.GetByIdAsync(request.ProductId);
            if (!exists.IsSuccess || exists.Value == null)
                return Result<Product>.Fail(exists.Errors ?? ["Produto não encontrado"]);
            
            var product = exists.Value;
            
            var increaseResult = product.IncreaseStock(request.Quantity);
            if (!increaseResult.IsSuccess)
                return Result<Product>.Fail(increaseResult.Errors!);
            
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