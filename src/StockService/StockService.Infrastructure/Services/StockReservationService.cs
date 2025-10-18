using SharedService.Shared;
using SharedService.Shared.Dtos;
using StockService.Application.Services;
using StockService.Domain.Repositories;

namespace StockService.Infrastructure.Services;

public class StockReservationService : IStockReservationService
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IProductCommandRepository _productCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockReservationService(
        IProductQueryRepository productQueryRepository, 
        IProductCommandRepository productCommandRepository, 
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _productCommandRepository = productCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> VerifyAndReserveStockAsync(Guid invoiceId, IEnumerable<InvoiceItemRequest> items)
    {
        var errors = new List<string>();

        try
        {
            await _unitOfWork.BeginTransactionAsync();
            
            foreach (var invoiceItem in items)
            {
                var productRepositoryResult = await _productQueryRepository.GetByIdAsync(invoiceItem.ProductId);
                if (!productRepositoryResult.IsSuccess || productRepositoryResult.Value == null)
                {
                    errors.Add("Produto não encontrado");
                    continue;
                }
                    
                var product = productRepositoryResult.Value!;
            
                var sufficientStock = product.HasSufficientStock(invoiceItem.Quantity);
                if (!sufficientStock)
                {
                    errors.Add(
                        $"{product.Name.Value}: Estoque insuficiente — {product.StockQuantity.Value} disponíveis");
                    
                    continue;
                }
                
                var decreaseResult = product.DecreaseStock(invoiceItem.Quantity);
                if (!decreaseResult.IsSuccess)
                {
                    errors.Add($"Erro ao reservar {product.Name.Value}: {decreaseResult.Errors?.FirstOrDefault()}");
                    continue;
                }
            
                await _productCommandRepository.UpdateAsync(product);
            }

            if (errors.Any())
            {
                await _unitOfWork.RollbackAsync();
                return Result<string>.Fail(errors);
            }
            
            await _unitOfWork.CommitAsync();
            
            return Result<string>.Ok("Produtos reservados com sucesso");
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            return Result<string>.Fail(["Ocorreu um erro ao reservar os produtos"]);
        }
    }
}