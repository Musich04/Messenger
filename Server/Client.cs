using System.Net.Sockets;


namespace TCPServer
{
    public class Client
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        protected internal StreamWriter Writer { get; }
        protected internal StreamReader Reader { get; }

        private TcpClient _client;
        private Server _server;

        public Client(TcpClient tcpClient, Server server)
        {
            _client = tcpClient;
            _server = server;

            Stream stream = _client.GetStream();
            Reader = new StreamReader(stream);
            Writer = new StreamWriter(stream);
        }

        public async Task ProcessAsync()
        {
            try
            {
                Console.WriteLine("Joining...");
                string? userName = await Reader.ReadLineAsync();
                string? message = $"{userName} joined to chat";
                Console.WriteLine("Joined");
                await _server.SendMessageAsync(Id, message);
                Console.WriteLine(message);

                while (true)
                {
                    try
                    {
                        if (message == null) continue;
                        message = await Reader.ReadLineAsync();
                        message = $"{userName}: {message}";
                        await _server.SendMessageAsync(Id, message);
                    }
                    catch
                    {
                        message = $"{userName} left the chat";
                        Console.WriteLine(message);
                        await _server.SendMessageAsync(Id, message);
                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                _server.RemoveConnection(Id);
            }
        }

        protected internal void Close()
        {
            Writer.Close();
            Reader.Close();
            _client.Close();
        }
    }
}
