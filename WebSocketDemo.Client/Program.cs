using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Uri serverUri = new Uri("ws://localhost:5208/ws"); // Match this with server port
        var webSocket = new ClientWebSocket();

        try
        {
            Console.WriteLine($"Attempting to connect to {serverUri}...");
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Connected to server.");

            // Send a message
            string message = "Hello from the client!";
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

            Console.WriteLine("Message sent to server.");

            // Receive a message
            var buffer = new byte[1024];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string serverMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received from server: {serverMessage}");

            // Close connection
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            Console.WriteLine("Connection closed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
