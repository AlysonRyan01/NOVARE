using Microsoft.AspNetCore.SignalR;

namespace InvoiceService.Infrastructure.Hubs;

public class InvoiceHub : Hub
{
    public async Task SendErrorMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveError", message);
    }
}