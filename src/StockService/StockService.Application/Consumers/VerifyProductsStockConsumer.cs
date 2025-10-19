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

        var stockResult = await _stockReservationService.VerifyAndReserveStockAsync(
            data.InvoiceId, 
            data.InvoiceItems
        );

        if (!stockResult.IsSuccess)
            await HandleOutOfStockAsync(data.InvoiceId, stockResult.Errors!);
        else
            await HandleStockReservedAsync(data.InvoiceId);
    }

    private Task HandleOutOfStockAsync(Guid invoiceId, IEnumerable<string> errors)
        => _eventPublisher.PublishOutOfStockAsync(new OutOfStockEvent(invoiceId, errors));

    private Task HandleStockReservedAsync(Guid invoiceId)
        => _eventPublisher.PublishStockReservedAsync(new StockReservedEvent(invoiceId));
}