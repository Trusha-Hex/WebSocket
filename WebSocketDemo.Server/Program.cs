using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

var webSocketClients = new List<WebSocket>();

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

// Enable WebSocket middleware
app.UseWebSockets();

app.Map("/ws", async context =>
{
    Console.WriteLine("Client attempting WebSocket connection...");
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine("WebSocket connection established.");
        webSocketClients.Add(webSocket);
        await EchoWebSocketHandler.HandleWebSocketAsync(webSocket, webSocketClients);
    }
    else
    {
        Console.WriteLine("WebSocket request failed.");
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});

app.Run();

public static class EchoWebSocketHandler
{
    public static async Task HandleWebSocketAsync(WebSocket webSocket, List<WebSocket> clients)
    {
        var buffer = new byte[1024];
        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Console.WriteLine($"Received: {message}");

                    // Send the message back to the same client (Echo)
                    await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Broadcast to all connected clients
                    await BroadcastMessage(message, clients);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            // Remove the client when disconnected
            clients.Remove(webSocket);
            Console.WriteLine("Client disconnected.");
        }
    }

    public static async Task BroadcastMessage(string message, List<WebSocket> clients)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        foreach (var client in clients)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
