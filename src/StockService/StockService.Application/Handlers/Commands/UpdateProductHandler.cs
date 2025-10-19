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

    public async Task<Result<Product>> Handle(UpdateProductCommand request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return Result<Product>.Fail(validationResult.Errors!);

        var productResult = await GetExistingProductAsync(request.Id, cancellationToken);
        if (!productResult.IsSuccess)
            return productResult;

        var product = productResult.Value!;
        var updateResult = UpdateProductFromCommand(request, product);
        if (!updateResult.IsSuccess)
            return updateResult;

        return await SaveProductAsync(product, cancellationToken);
    }

    private async Task<Result> ValidateRequestAsync(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var result = await _validator.ValidateAsync(request, cancellationToken);
        if (!result.IsValid)
            return Result.Fail(result.Errors.Select(x => x.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<Product>> GetExistingProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        var productResult = await _productQueryRepository.GetByIdAsync(productId, cancellationToken);
        if (!productResult.IsSuccess)
            return Result<Product>.Fail(productResult.Errors!);

        if (productResult.Value == null)
            return Result<Product>.Fail(["Produto não encontrado"]);

        return Result<Product>.Ok(productResult.Value);
    }

    private Result<Product> UpdateProductFromCommand(UpdateProductCommand request, Product product)
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

        return product.UpdateBasicInfo(
            nameResult.Value!,
            descriptionResult.Value!,
            priceResult.Value!
        );
    }

    private async Task<Result<Product>> SaveProductAsync(Product product, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var repoResult = await _productCommandRepository.UpdateAsync(product, cancellationToken);
            if (!repoResult.IsSuccess)
                return Result<Product>.Fail(repoResult.Errors!);

            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
            if (rowsAffected == 0)
                return Result<Product>.Fail(["Nenhum registro foi atualizado"]);

            return Result<Product>.Ok(product);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Product>.Fail([$"Erro ao atualizar produto: {ex.Message}"]);
        }
    }
}
