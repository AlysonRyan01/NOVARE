using InvoiceService.Application.Services;
using InvoiceService.Domain.Repositories.Invoices;
using InvoiceService.Infrastructure.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SharedService.Shared.Events;

namespace InvoiceService.Infrastructure.Consumers;

public class StockReservedConsumer : IConsumer<StockReservedEvent>
{
    private readonly ILogger<StockReservedConsumer> _logger;
    private readonly IInvoiceQueryRepository _invoiceQueryRepository;
    private readonly IInvoiceCommandRepository _invoiceCommandRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<InvoiceHub> _hubContext;


    public StockReservedConsumer(
        ILogger<StockReservedConsumer> logger,
        IInvoiceQueryRepository invoiceQueryRepository,
        IInvoiceCommandRepository invoiceCommandRepository,
        IUnitOfWork unitOfWork,
        IHubContext<InvoiceHub> hubContext)
    {
        _logger = logger;
        _invoiceQueryRepository = invoiceQueryRepository;
        _invoiceCommandRepository = invoiceCommandRepository;
        _unitOfWork = unitOfWork;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var @event = context.Message;

        try
        {
            _logger.LogInformation(
                "Processando StockReservedEvent para Invoice: {InvoiceId}",
                @event.InvoiceId);
            
            var invoiceResult = await _invoiceQueryRepository.GetByIdAsync(@event.InvoiceId);
            if (!invoiceResult.IsSuccess || invoiceResult.Value == null)
                return;
            
            var invoice = invoiceResult.Value;
            
            invoice.RebuildState();
            
            var result = invoice.MarkAsPrinted();
            if (!result.IsSuccess)
            {
                _logger.LogError(
                    "Erro ao marcar invoice como impressa: {Errors}",
                    string.Join(", ", result.Errors!));
                return;
            }
            
            await _unitOfWork.BeginTransactionAsync();
            await _invoiceCommandRepository.UpdateAsync(invoice);
            await _unitOfWork.CommitAsync();
            
            await _hubContext.Clients.All.SendAsync(
                "ReceiveSuccess",
                "Nota fiscal impressa com sucesso!");

            _logger.LogInformation(
                "Invoice {InvoiceId} marcada como impressa com sucesso e notificação SignalR GLOBAL enviada",
                @event.InvoiceId);
        }
        catch (Exception ex)
        {
            await _hubContext.Clients.All.SendAsync(
                "ReceiveError",
                "Erro crítico ao processar a nota fiscal");
            _logger.LogError(
                ex,
                "Erro ao processar StockReservedEvent para Invoice: {InvoiceId}",
                @event.InvoiceId);
            throw;
        }
    }
}