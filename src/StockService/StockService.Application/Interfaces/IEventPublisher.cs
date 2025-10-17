using SharedService.Shared.Dtos;
using SharedService.Shared.Events;

namespace ProductService.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishStockReservedAsync(StockReservedEvent @event);
    Task PublishOutOfStockAsync(OutOfStockEvent @event);
}