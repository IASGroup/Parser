using Microsoft.AspNetCore.SignalR;

namespace Hubs.Hubs;

public class ParseTasksHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"Connected {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Disconnect {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
}