using FluentValidation;
using MediatR;
using SharedService.Shared;
using StockService.Application.Commands;
using StockService.Application.Services;
using StockService.Domain.Repositories;

namespace StockService.Application.Handlers.Commands;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand,  Result<Guid>>
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

    public async Task<Result<Guid>> Handle(
        DeleteProductCommand request, 
        CancellationToken cancellationToken = default)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if  (!validationResult.IsValid)
            return Result<Guid>.Fail(validationResult.Errors.Select(x => x.ErrorMessage).ToList());

        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var existsResult = await _productQueryRepository.ExistsAsync(request.Id, cancellationToken);
            if (!existsResult.IsSuccess)
                return Result<Guid>.Fail(existsResult.Errors!);
            
            if (!existsResult.Value)
                return Result<Guid>.Fail(["Produto não encontrado"]);
            
            var deleteResult = await _productCommandRepository.DeleteAsync(request.Id, cancellationToken);
            if (!deleteResult.IsSuccess)
                return Result<Guid>.Fail(deleteResult.Errors!);
            
            var rowsAffected =  await _unitOfWork.CommitAsync(cancellationToken);
            if  (rowsAffected == 0)
                return Result<Guid>.Fail(["Ocorreu um erro ao deletar um produto"]);
            
            return Result<Guid>.Ok(request.Id);
        }
        catch 
        {
            await _unitOfWork.RollbackAsync(cancellationToken);
            return Result<Guid>.Fail(["Ocorreu um erro no servidor"]);
        }
    }
}