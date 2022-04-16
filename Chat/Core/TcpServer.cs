using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Core
{
    public class TcpServer
    {
        private readonly List<Socket> _clients = new List<Socket>();

        private readonly IConsole         _console;

        private Socket _listener;

        public TcpServer(IConsole console)
        {
            _console = console;
        }

        public void Run()
        {
            IPAddress  ipAddress     = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                _console.LogSuccess( "Server Successfully started");
                _console.Log( "Waiting for connection");
                
                AcceptHandler();
            }
            catch (Exception e)
            {
                _console.LogError( "SocketException : " + e);
                throw;
            }
        }

        private async Task AcceptHandler()
        {
            while (true)
            {
                Socket client = await _listener.AcceptAsync();
                _console.LogSuccess("New client connected!");
                _clients.Add(client);
                ReceiveHandler(client);
            }
        }

        private async Task ReceiveHandler(Socket client)
        {
            while (true)
            {
                try
                {
                    Packet             packet       = new Packet();
                    ArraySegment<byte> arraySegment = new ArraySegment<byte>(packet.Buffer, 0, Packet.BufferSize);
                    int                numBytesRead = await client.ReceiveAsync(arraySegment, SocketFlags.None);
                    byte[]             data         = arraySegment.ToArray();

                    StringBuilder sb = new StringBuilder();
                    sb.Append(Encoding.Default.GetString(data, 1, numBytesRead - 1));
                    _console.Log( sb.ToString());
                    await UpdateAllClients(client, data);
                }
                catch (Exception)
                {
                    _console.LogWarning( "Client has forcefully disconnected!");
                    CloseHandler(client);
                }
            }
        }
        
        private void CloseHandler(Socket handler)
        {
            if (handler == null) { return; }

            _clients.Remove(handler);
            try
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception) { _console.LogWarning( "Failed to close the clients handler!"); }
        }
        
        private async Task UpdateAllClients(Socket handler, byte[] bytes)
        {
            foreach (Socket client in _clients.Where(client => !client.Equals(handler)))
            {
                await Send(client, bytes);
            }
        }

        private async Task Send(Socket client, byte[] bytes)
        {
            await client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None);
        }
    }
}