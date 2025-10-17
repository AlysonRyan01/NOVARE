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

    public async Task Handle(InvoicePrintingRequestedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Publicando evento VerifyProductsStockEvent para Invoice: {InvoiceId}",
                notification.InvoiceId);
            
            var invoiceItems = notification.Items.Select(item => new InvoiceItemRequest(
                item.ProductId,
                item.Quantity
            ));
            
            var verifyStockEvent = new VerifyProductsStockEvent(
                notification.InvoiceId, 
                invoiceItems);
            
            await _publishEndpoint.Publish(verifyStockEvent, cancellationToken);

            _logger.LogInformation(
                "Evento VerifyProductsStockEvent publicado com sucesso para Invoice: {InvoiceId}",
                notification.InvoiceId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao publicar VerifyProductsStockEvent para Invoice: {InvoiceId}",
                notification.InvoiceId);
            throw;
        }
    }
}