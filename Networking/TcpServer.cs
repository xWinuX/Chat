using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networking.Packets;

namespace Networking
{
    public class TcpServer
    {
        private readonly List<Socket> _clients = new List<Socket>();
        
        private Socket _listener;

        protected readonly IConsole Console;
        
        public TcpServer(IConsole console) { Console = console; }

        public async void Run()
        {
            IPAddress  ipAddress     = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                Console.LogSuccess("Server Successfully started");
                Console.Log("Waiting for connection");

                await AcceptHandler();
            }
            catch (Exception e)
            {
                Console.LogError("SocketException : " + e);
                throw;
            }
        }

        private async Task AcceptHandler()
        {
            while (true)
            {
                Socket client = await _listener.AcceptAsync();
                Console.LogSuccess("New client connected!");
                _clients.Add(client);
                ReceiveHandler(client);
            }
        }

        private async Task ReceiveHandler(Socket client)
        {
            try
            {
                while (true)
                {
                    byte[]             bytes        = new byte[Packet.BufferSize];
                    ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes);
                    int                numBytesRead = await client.ReceiveAsync(arraySegment, SocketFlags.None);
                    byte[]             data         = arraySegment.ToArray();

                    try { await ResolvePacket(data, numBytesRead); }
                    catch (Exception e)
                    {
                        Console.LogError("An Error occured while trying to parse a packet!");
                        Console.LogError(e.ToString());
                        throw;
                    }
                    
                }
            }
            catch (Exception)
            {
                Console.LogWarning("Client has forcefully disconnected!");
                CloseHandler(client);
            }
        }

        protected virtual async Task ResolvePacket(byte[] data, int numBytesRead) { }

        private void CloseHandler(Socket handler)
        {
            if (handler == null) { return; }

            _clients.Remove(handler);
            try
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception) { Console.LogWarning("Failed to close the clients handler!"); }
        }

        protected async Task SendToConnectedClients(byte[] bytes)
        {
            foreach (Socket client in _clients) { await Send(client, bytes); }
        }

        private async Task Send(Socket client, byte[] bytes) { await client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None); }
    }
}