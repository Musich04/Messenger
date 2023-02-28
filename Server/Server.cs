using System.Net;
using System.Net.Sockets;


namespace TCPServer
{
    public class Server
    {
        private TcpListener _listener = new TcpListener(IPAddress.Any, 9000);
        private List<Client> _clients = new List<Client>();

        protected internal void RemoveConnection(string id)
        {
            Client? client = _clients.FirstOrDefault(cl => cl.Id == id);

            if (client != null)
                _clients.Remove(client);

            client?.Close();
        }

        protected internal async Task StartAsync()
        {
            try
            {
                _listener.Start();
                Console.WriteLine("Server started. Wait...");

                while (true)
                {

                    TcpClient? tcpClient = await _listener.AcceptTcpClientAsync();

                    Client client = new Client(tcpClient, this);
                    _clients.Add(client);
                    Console.WriteLine($"New Client: {client.Id}");
                    Task.Run(client.ProcessAsync).Wait();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Disconnect();
            }
        }

        protected internal async Task SendMessageAsync(string id, string message)
        {
            foreach (Client client in _clients)
            {
                if (client.Id != id)
                {
                    await client.Writer.WriteAsync(message);
                    await client.Writer.FlushAsync();
                }
            }
        }

        protected internal void Disconnect()
        {
            foreach (Client client in _clients)
            {
                client.Close();
            }

            _listener.Stop();
        }

    }
}
