using FluentValidation;
using MediatR;
using SharedService.Shared;
using StockService.Application.Commands;
using StockService.Application.Services;
using StockService.Domain.Repositories;

namespace StockService.Application.Handlers.Commands;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result<Guid>>
{
    private readonly IProductCommandRepository _productCommandRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<DeleteProductCommand> _validator;

    public DeleteProductHandler(
        IProductCommandRepository productCommandRepository, 
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork, 
        IValidator<DeleteProductCommand> validator)
    {
        _productCommandRepository = productCommandRepository;
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Result<Guid>> Handle(DeleteProductCommand request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateRequestAsync(request, cancellationToken);
        if (!validationResult.IsSuccess)
            return Result<Guid>.Fail(validationResult.Errors!);

        var existsResult = await CheckIfProductExistsAsync(request.Id, cancellationToken);
        if (!existsResult.IsSuccess)
            return Result<Guid>.Fail(existsResult.Errors!);

        return await DeleteProductAsync(request.Id, cancellationToken);
    }

    private async Task<Result> ValidateRequestAsync(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
            return Result.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        return Result.Ok();
    }

    private async Task<Result<bool>> CheckIfProductExistsAsync(Guid productId, CancellationToken cancellationToken)
    {
        var existsResult = await _productQueryRepository.ExistsAsync(productId, cancellationToken);
        if (!existsResult.IsSuccess)
            return Result<bool>.Fail(existsResult.Errors!);

        if (!existsResult.Value)
            return Result<bool>.Fail(["Produto não encontrado"]);

        return Result<bool>.Ok(true);
    }

    private async Task<Result<Guid>> DeleteProductAsync(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var deleteResult = await _productCommandRepository.DeleteAsync(productId, cancellationToken);
            if (!deleteResult.IsSuccess)
                return Result<Guid>.Fail(deleteResult.Errors!);

            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
            if (rowsAffected == 0)
                return Result<Guid>.Fail(["Ocorreu um erro ao deletar o produto"]);

            return Result<Guid>.Ok(productId);
        }
        catch
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Guid>.Fail(["Ocorreu um erro no servidor"]);
        }
    }
}
