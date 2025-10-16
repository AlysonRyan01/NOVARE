using FluentValidation;
using MediatR;
using SharedService.Shared;
using StockService.Application.Commands;
using StockService.Application.Services;
using StockService.Domain.Entities;
using StockService.Domain.Repositories;
using StockService.Domain.ValueObjects.Product;

namespace StockService.Application.Handlers.Commands;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<Product>>
{
    private readonly IProductCommandRepository _productCommandRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<UpdateProductCommand> _validator;
    
    public UpdateProductHandler(
        IProductCommandRepository productCommandRepository,
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork,
        IValidator<UpdateProductCommand> validator)
    {
        _productCommandRepository = productCommandRepository;
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Product>> Handle(
        UpdateProductCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result<Product>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var existingProductResult = await _productQueryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (!existingProductResult.IsSuccess)
                return Result<Product>.Fail(existingProductResult.Errors!);

            var existingProduct = existingProductResult.Value;
            if (existingProduct == null)
                return Result<Product>.Fail(["Produto não encontrado"]);

            var updateResult = UpdateProductFromCommand(request, existingProduct);
            if (!updateResult.IsSuccess)
                return updateResult;
            
            var repositoryResult = await _productCommandRepository.UpdateAsync(existingProduct, cancellationToken);
            if (!repositoryResult.IsSuccess)
                return Result<Product>.Fail(repositoryResult.Errors!);
            
            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
            if (rowsAffected == 0)
                return Result<Product>.Fail(["Nenhum registro foi atualizado"]);

            return Result<Product>.Ok(existingProduct);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Product>.Fail([$"Erro ao atualizar produto: {ex.Message}"]);
        }
    }

    private Result<Product> UpdateProductFromCommand(UpdateProductCommand request, Product existingProduct)
    {
        var nameResult = Name.Create(request.Name);
        var descriptionResult = Description.Create(request.Description);
        var priceResult = Price.Create(request.Price);
            
        var errors = new List<string>();
        if (!nameResult.IsSuccess) errors.AddRange(nameResult.Errors!);
        if (!descriptionResult.IsSuccess) errors.AddRange(descriptionResult.Errors!);
        if (!priceResult.IsSuccess) errors.AddRange(priceResult.Errors!);
            
        if (errors.Any())
            return Result<Product>.Fail(errors);
            
        return existingProduct.UpdateBasicInfo(
            nameResult.Value!,
            descriptionResult.Value!,
            priceResult.Value!
        );
    }
}