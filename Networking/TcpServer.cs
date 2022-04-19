using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networking.Packets;

namespace Networking
{
    public abstract class TcpServer : SocketBehaviour
    {
        protected readonly IConsole Console;

        private readonly List<Socket> _clients = new List<Socket>();

        private Socket _listener;

        protected TcpServer(string address, int port, IConsole console) : base(address, port) { Console = console; }

        /// <summary>
        /// Runs the server
        /// </summary>
        public async void Run()
        {
            IPAddress  ipAddress     = IPAddress.Parse(Address);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                Console.LogSuccess("Server Successfully started");
                Console.Log("Waiting for connection");

                AcceptHandler();
            }
            catch (Exception e)
            {
                Console.LogError("Server start failed!");
                Console.LogError(e.ToString());
                throw;
            }
        }

        protected void CloseClient(Socket client)
        {
            Console.Log("Closing client connection...");

            if (client == null) { return; }

            _clients.Remove(client);

            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Console.Log("Finished closing client connection!");
            }
            catch (Exception) { Console.LogWarning("Failed to close the clients handler!"); }
        }

        protected async Task SendToConnectedClients(byte[] bytes)
        {
            foreach (Socket client in _clients) { await Send(client, bytes); }
        }

        protected abstract Task<bool> ResolvePacket(Socket client, byte[] data, int numBytesRead);

        private async Task AcceptHandler()
        {
            try
            {
                while (true)
                {
                    Socket client = await _listener.AcceptAsync();
                    Console.LogSuccess("New client connected!");
                    _clients.Add(client);
                    ReceiveHandler(client);
                }
            }
            catch (Exception e)
            {
                Console.LogError("Accept Error");
                Console.LogError(e.ToString());
                throw;
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

                    bool stayConnected;
                    try { stayConnected = await ResolvePacket(client, data, numBytesRead); }
                    catch (Exception e)
                    {
                        Console.LogError("An Error occured while trying to parse a packet!");
                        Console.LogError(e.ToString());
                        throw;
                    }

                    Console.Log(stayConnected.ToString());

                    if (!stayConnected) { break; }
                }
            }
            catch (Exception)
            {
                Console.LogWarning("Client has forcefully disconnected!");
                CloseClient(client);
            }
        }

        private async Task Send(Socket client, byte[] bytes) { await client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None); }
    }
}