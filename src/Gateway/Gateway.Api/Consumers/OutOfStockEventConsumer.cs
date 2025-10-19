using Gateway.Api.Hubs;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SharedService.Shared.Events;

namespace Gateway.Api.Consumers;

public class OutOfStockEventConsumer : IConsumer<OutOfStockNotifier>
{
    private readonly ILogger<StockReservedEventConsumer> _logger;
    private readonly IHubContext<GatewayHub> _hubContext;

    public OutOfStockEventConsumer(
        ILogger<StockReservedEventConsumer> logger, 
        IHubContext<GatewayHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<OutOfStockNotifier> context)
    {
        var body = context.Message;
        
        _logger.LogInformation("Enviando a notificação sobre a falta de estoque de produto");
        await _hubContext.Clients.All.SendAsync("ReceiveError", string.Join(", ", body.Errors));
    }
}