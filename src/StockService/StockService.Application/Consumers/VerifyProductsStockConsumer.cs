using MassTransit;
using ProductService.Application.Interfaces;
using SharedService.Shared.Events;
using StockService.Application.Services;

namespace ProductService.Application.Consumers;

public class VerifyProductsStockConsumer : IConsumer<VerifyProductsStockEvent>
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IStockReservationService _stockReservationService;

    public VerifyProductsStockConsumer(
        IEventPublisher eventPublisher,
        IStockReservationService stockReservationService)
    {
        _eventPublisher = eventPublisher;
        _stockReservationService = stockReservationService;
    }

    public async Task Consume(ConsumeContext<VerifyProductsStockEvent> context)
    {
        var data = context.Message;
        var products = data.InvoiceItems;

        var stockResult = await _stockReservationService.VerifyAndReserveStockAsync(data.InvoiceId, products);
        if (!stockResult.IsSuccess)
        {
            await _eventPublisher.PublishOutOfStockAsync(new OutOfStockEvent(data.InvoiceId, stockResult.Errors!));
            return;
        }

        await _eventPublisher.PublishStockReservedAsync(new StockReservedEvent(data.InvoiceId));
    }
}