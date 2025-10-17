using SharedService.Shared;
using SharedService.Shared.Dtos;

namespace StockService.Application.Services;

public interface IStockReservationService
{
    Task<Result<string>> VerifyAndReserveStockAsync(Guid invoiceId, IEnumerable<InvoiceItemRequest> items);
}