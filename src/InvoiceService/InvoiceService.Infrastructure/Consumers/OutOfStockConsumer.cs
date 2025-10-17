using InvoiceService.Application.Services;
using InvoiceService.Domain.Repositories.Invoices;
using MassTransit;
using Microsoft.Extensions.Logging;
using SharedService.Shared.Events;

namespace InvoiceService.Infrastructure.Consumers;

public class OutOfStockConsumer : IConsumer<OutOfStockEvent>
{
    private readonly ILogger<OutOfStockConsumer> _logger;
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public OutOfStockConsumer(
        ILogger<OutOfStockConsumer> logger,
        IInvoiceQueryRepository invoiceQueryRepository,
        IInvoiceCommandRepository invoiceCommandRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _invoiceQueryRepository = invoiceQueryRepository;
        _invoiceCommandRepository = invoiceCommandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Consume(ConsumeContext<OutOfStockEvent> context)
    {
        var @event = context.Message;

        try
        {
            _logger.LogInformation(
                "Processando OutOfStockEvent para Invoice: {InvoiceId}",
                @event.InvoiceId);
            
            var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(@event.InvoiceId);
            if (!invoiceResult.IsSuccess || invoiceResult.Value == null)
                return;
            
            var invoice = invoiceResult.Value;
            
            invoice.RebuildState();
            
            var result = invoice.MarkAsOutOfStock(@event.Errors.ToList());
            if (!result.IsSuccess)
            {
                _logger.LogError(
                    "Erro ao marcar invoice como out of stock: {Errors}",
                    string.Join(", ", result.Errors!));
                return;
            }
            
            await _unitOfWork.BeginTransactionAsync();
            await _invoiceCommandRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync();
            
            _logger.LogInformation(
                "Invoice {InvoiceId} marcada como out of stock e notificação enviada. Erros: {Errors}",
                @event.InvoiceId,
                string.Join(", ", @event.Errors));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao processar OutOfStockEvent para Invoice: {InvoiceId}",
                @event.InvoiceId);
            throw;
        }
    }
}