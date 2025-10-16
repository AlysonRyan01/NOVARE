using FluentValidation;
using MediatR;
using SharedService.Shared;
using StockService.Application.Commands;
using StockService.Application.Services;
using StockService.Domain.Builders;
using StockService.Domain.Entities;
using StockService.Domain.Repositories;
using StockService.Domain.ValueObjects.Product;

namespace StockService.Application.Handlers.Commands;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<Product>>
{
    private readonly IProductCommandRepository _productCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateProductCommand> _validator;

    public CreateProductHandler(
        IProductCommandRepository productCommandRepository, 
        IUnitOfWork unitOfWork, 
        IValidator<CreateProductCommand> validator)
    {
        _productCommandRepository = productCommandRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Product>> Handle(
        CreateProductCommand request, 
        CancellationToken cancellationToken =  default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<Product>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var productBuildResult = CreateProductFromCommand(request);
            
            if (!productBuildResult.IsSuccess)
                return Result<Product>.Fail(productBuildResult.Errors!);
            
            var product = productBuildResult.Value;
            if (product == null)
                return Result<Product>.Fail(["Erro ao criar o produto"]);
            
            var repositoryResult =  await _productCommandRepository.CreateAsync(product, cancellationToken);
            if (!repositoryResult.IsSuccess)
                return Result<Product>.Fail(repositoryResult.Errors!);

            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
            if (rowsAffected == 0)
                return Result<Product>.Fail(["Erro ao adicionar o produto"]);
                
            return Result<Product>.Ok(product);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Product>.Fail(["Erro interno no servidor"]);
        }
    }

    private Result<Product> CreateProductFromCommand(CreateProductCommand request)
    {
        var nameResult = Name.Create(request.Name);
        var descriptionResult = Description.Create(request.Description);
        var priceResult = Price.Create(request.Price);
        var stockResult = StockQuantity.Create(request.Quantity);
        
        var errors = new List<string>();
        if (!nameResult.IsSuccess) errors.AddRange(nameResult.Errors!);
        if (!descriptionResult.IsSuccess) errors.AddRange(descriptionResult.Errors!);
        if (!priceResult.IsSuccess) errors.AddRange(priceResult.Errors!);
        if (!stockResult.IsSuccess) errors.AddRange(stockResult.Errors!);
    
        if (errors.Any())
            return Result<Product>.Fail(errors);

        return new ProductBuilder()
            .WithName(nameResult.Value!)
            .WithDescription(descriptionResult.Value!)
            .WithPrice(priceResult.Value!)
            .WithStockQuantity(stockResult.Value!)
            .Build();
    }
}