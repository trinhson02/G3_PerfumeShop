using Microsoft.AspNetCore.SignalR;

namespace G3_PerfumeShop.SignalR.Hubs
{
    public class ChatHub: Hub
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
