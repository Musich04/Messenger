using System;
using System.Net;
using System.Net.Sockets;

string ip = "127.0.0.1";
int port = 9000;

TcpClient tcpClient = new TcpClient();
Console.WriteLine("Write your name: ");
string? userName = Console.ReadLine();
Console.WriteLine($"Welcome home, {userName}");
StreamReader? Reader = null;
StreamWriter? Writer = null;

try
{
    tcpClient.Connect(ip, port);
    Reader = new StreamReader(tcpClient.GetStream());
    Writer = new StreamWriter(tcpClient.GetStream());

    if (Reader == null || Writer == null)
        return;

    await SendMessageAsync(Writer);
    Task.Run(() => ReceiveMessageAsync(Reader)).Wait();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Writer?.Close();
Reader?.Close();


async Task SendMessageAsync(StreamWriter writer)
{
    await writer.WriteLineAsync(userName);
    await writer.FlushAsync();
    Console.WriteLine("Type a message and press Enter to send");

    while (true)
    {
        string? message = Console.ReadLine();
        await writer.WriteLineAsync(message);
        await writer.FlushAsync();
    }

}

async Task ReceiveMessageAsync(StreamReader reader)
{
    while (true)
    {
        try
        {
            string? message = await reader.ReadLineAsync();

            if (string.IsNullOrEmpty(message))
                continue;

            Console.WriteLine(message);
        }
        catch
        {
            break;
        }
    }
}