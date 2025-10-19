using InvoiceService.Application.Services;
using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Repositories.Invoices;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedService.Shared;
using SharedService.Shared.Events;

namespace InvoiceService.Infrastructure.Consumers;

public class OutOfStockConsumer : IConsumer<OutOfStockEvent>
{
    private readonly ILogger<OutOfStockConsumer> _logger;
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public OutOfStockConsumer(
        ILogger<OutOfStockConsumer> logger,
        IInvoiceQueryRepository invoiceQueryRepository,
        IInvoiceCommandRepository invoiceCommandRepository,
        IUnitOfWork unitOfWork,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _invoiceQueryRepository = invoiceQueryRepository;
        _invoiceCommandRepository = invoiceCommandRepository;
        _unitOfWork = unitOfWork;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<OutOfStockEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation("Processando OutOfStockEvent para Invoice: {InvoiceId}", @event.InvoiceId);

        var invoiceResult = await LoadInvoiceAsync(@event.InvoiceId);
        if (!invoiceResult.IsSuccess)
            return;

        var invoice = invoiceResult.Value!;
        invoice.RebuildState();

        var markResult = MarkInvoiceOutOfStock(invoice, @event);
        if (!markResult.IsSuccess)
        {
            _logger.LogError("Erro ao marcar invoice como out of stock: {Errors}", string.Join(", ", markResult.Errors!));
            return;
        }

        await PersistAndNotifyAsync(invoice, @event);
    }

    private async Task<Result<Invoice>> LoadInvoiceAsync(Guid invoiceId)
    {
        var result = await _invoiceQueryRepository.GetByIdAsync(invoiceId);
        if (!result.IsSuccess || result.Value == null)
            return Result<Invoice>.Fail(result.Errors!);

        return Result<Invoice>.Ok(result.Value);
    }

    private Result<Invoice> MarkInvoiceOutOfStock(Invoice invoice, OutOfStockEvent @event)
    {
        return invoice.MarkAsOutOfStock(@event.Errors.ToList());
    }

    private async Task PersistAndNotifyAsync(Invoice invoice, OutOfStockEvent @event)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _invoiceCommandRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync();
            
            var notifier = new OutOfStockNotifier(@event.InvoiceId, @event.Errors.ToList());

            await _publishEndpoint.Publish(notifier);

            _logger.LogInformation(
                "Invoice {InvoiceId} marcada como out of stock e notificação enviada. Erros: {Errors}",
                @event.InvoiceId,
                string.Join(", ", @event.Errors));
        }
        catch (Exception ex)
        {
            var notifier = new OutOfStockNotifier(
                @event.InvoiceId, [$"Erro crítico ao processar a nota fiscal {@event.InvoiceId}"]);
            
            await _publishEndpoint.Publish(notifier);
            _logger.LogError(ex, "Erro ao processar OutOfStockEvent para Invoice: {InvoiceId}", @event.InvoiceId);
            throw;
        }
    }
}
