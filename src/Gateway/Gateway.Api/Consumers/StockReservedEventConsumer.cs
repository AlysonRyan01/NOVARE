using Gateway.Api.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SharedService.Shared.Events;

namespace Gateway.Api.Consumers;

public class StockReservedEventConsumer : IConsumer<StockReservedNotifier>
{
    private readonly ILogger<StockReservedEventConsumer> _logger;
    private readonly IHubContext<GatewayHub> _hubContext;

    public StockReservedEventConsumer(
        ILogger<StockReservedEventConsumer> logger, 
        IHubContext<GatewayHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<StockReservedNotifier> context)
    {
        _logger.LogInformation("Processando a notificação da reserva de estoque");
        
        await _hubContext.Clients.All.SendAsync("ReceiveSuccess", "Nota fiscal impressa com sucesso!");
    }
}