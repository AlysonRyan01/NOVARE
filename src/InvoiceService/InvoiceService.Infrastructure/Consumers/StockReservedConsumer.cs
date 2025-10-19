using InvoiceService.Application.Services;
using InvoiceService.Domain.AggregateRoots;
using InvoiceService.Domain.Repositories.Invoices;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedService.Shared;
using SharedService.Shared.Events;

namespace InvoiceService.Infrastructure.Consumers;

public class StockReservedConsumer : IConsumer<StockReservedEvent>
{
    private readonly ILogger<StockReservedConsumer> _logger;
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPublishEndpoint _publishEndpoint;

    public StockReservedConsumer(
        ILogger<StockReservedConsumer> logger,
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

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var @event = context.Message;
        _logger.LogInformation("Processando StockReservedEvent para Invoice: {InvoiceId}", @event.InvoiceId);

        var invoiceResult = await LoadInvoiceAsync(@event.InvoiceId);
        if (!invoiceResult.IsSuccess)
            return;

        var invoice = invoiceResult.Value!;
        invoice.RebuildState();

        var markResult = MarkInvoicePrinted(invoice);
        if (!markResult.IsSuccess)
        {
            _logger.LogError("Erro ao marcar invoice como impressa: {Errors}", string.Join(", ", markResult.Errors!));
            return;
        }

        await PersistAndNotifyAsync(invoice);
    }

    private async Task<Result<Invoice>> LoadInvoiceAsync(Guid invoiceId)
    {
        var result = await _invoiceQueryRepository.GetByIdAsync(invoiceId);
        if (!result.IsSuccess || result.Value == null)
            return Result<Invoice>.Fail(result.Errors!);

        return Result<Invoice>.Ok(result.Value);
    }

    private Result<Invoice> MarkInvoicePrinted(Invoice invoice)
    {
        return invoice.MarkAsPrinted();
    }

    private async Task PersistAndNotifyAsync(Invoice invoice)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await _invoiceCommandRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync();

            var notifier = new StockReservedNotifier();
            await _publishEndpoint.Publish(notifier);

            _logger.LogInformation(
                "Invoice {InvoiceId} marcada como impressa com sucesso e notificação SignalR GLOBAL enviada",
                invoice.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao persistir invoice {InvoiceId}", invoice.Id);
            throw;
        }
    }
}
