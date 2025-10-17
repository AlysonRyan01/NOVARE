using MassTransit;
using ProductService.Application.Interfaces;
using SharedService.Shared.Events;

namespace ProductService.Infrastructure.EventPublishers;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public EventPublisher(
        IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }
    
    public async Task PublishStockReservedAsync(StockReservedEvent @event)
    {
        await _publishEndpoint.Publish(@event);
    }

    public async Task PublishOutOfStockAsync(OutOfStockEvent @event)
    {
        await _publishEndpoint.Publish(@event);
    }
}