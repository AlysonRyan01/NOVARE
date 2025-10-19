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
                var result = await ReserveItemAsync(invoiceItem);
                if (!result.IsSuccess)
                    errors.AddRange(result.Errors!);
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

    private async Task<Result> ReserveItemAsync(InvoiceItemRequest invoiceItem)
    {
        var productResult = await _productQueryRepository.GetByIdAsync(invoiceItem.ProductId);
        if (!productResult.IsSuccess || productResult.Value == null)
            return Result.Fail(new[] { "Produto não encontrado" });

        var product = productResult.Value;

        if (!product.HasSufficientStock(invoiceItem.Quantity))
            return Result.Fail(new[] { $"{product.Name.Value}: Estoque insuficiente — {product.StockQuantity.Value} disponíveis" });

        var decreaseResult = product.DecreaseStock(invoiceItem.Quantity);
        if (!decreaseResult.IsSuccess)
            return Result.Fail(decreaseResult.Errors!);

        await _productCommandRepository.UpdateAsync(product);
        return Result.Ok();
    }
}