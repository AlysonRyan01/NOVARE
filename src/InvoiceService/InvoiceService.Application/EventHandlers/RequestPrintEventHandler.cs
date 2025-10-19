using InvoiceService.Domain.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedService.Shared.Dtos;
using SharedService.Shared.Events;

namespace InvoiceService.Application.EventHandlers;

public class RequestPrintEventHandler : INotificationHandler<InvoicePrintingRequestedEvent>
{
    private readonly ILogger<RequestPrintEventHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public RequestPrintEventHandler(
        ILogger<RequestPrintEventHandler> logger,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(InvoicePrintingRequestedEvent notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando publicação do evento VerifyProductsStockEvent para Invoice: {InvoiceId}", notification.InvoiceId);

        var verifyStockEvent = BuildVerifyStockEvent(notification);

        await PublishEventAsync(verifyStockEvent, cancellationToken);

        _logger.LogInformation("Evento VerifyProductsStockEvent publicado com sucesso para Invoice: {InvoiceId}", notification.InvoiceId);
    }

    private static VerifyProductsStockEvent BuildVerifyStockEvent(InvoicePrintingRequestedEvent notification)
    {
        var invoiceItems = notification.Items
            .Select(item => new InvoiceItemRequest(item.ProductId, item.Quantity));

        return new VerifyProductsStockEvent(notification.InvoiceId, invoiceItems);
    }

    private async Task PublishEventAsync(
        VerifyProductsStockEvent verifyStockEvent, 
        CancellationToken cancellationToken)
    {
        await _publishEndpoint.Publish(verifyStockEvent, cancellationToken);
    }
}