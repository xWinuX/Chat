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
        public void Run()
        {
            IPAddress  ipAddress     = IPAddress.Parse(Address);
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Port);
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                Console.Log("Trying to bind server...");
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                Console.LogSuccess("Server Successfully started");
                Console.Log("Waiting for connection...");

                AcceptHandler();
            }
            catch (Exception e)
            {
                Console.LogError("Server start failed!");
                Console.LogError(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Closes given client and removes it from the client lsit
        /// </summary>
        /// <param name="client">Client to close</param>
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

        /// <summary>
        /// Send given byte array to all connected clients
        /// </summary>
        /// <param name="bytes">Byte array to send</param>
        protected async Task SendToConnectedClients(byte[] bytes)
        {
            foreach (Socket client in _clients) { await Send(client, bytes); }
        }

        /// <summary>
        /// Resolves given data
        /// </summary>
        /// <param name="client">Client that sent this data</param>
        /// <param name="data">Byte array to resolve</param>
        /// <param name="numBytesRead">Number of bytes read</param>
        /// <returns>Task resolving in a boolean, true means the client will stay connected, false means it will not</returns>
        protected abstract Task<bool> ResolvePacket(Socket client, byte[] data, int numBytesRead);

        /// <summary>
        /// Handles accepting connections
        /// </summary>
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

        /// <summary>
        /// Handles receiving data
        /// </summary>
        /// <param name="client">Client that sent the data</param>
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

                    if (!stayConnected) { break; }
                }
            }
            catch (Exception)
            {
                Console.LogWarning("Client has forcefully disconnected!");
                CloseClient(client);
            }
        }

        /// <summary>
        /// Send give byte array to given client
        /// </summary>
        /// <param name="client">Client to send data to</param>
        /// <param name="bytes">Byte array to send to client</param>
        private static async Task Send(Socket client, byte[] bytes) { await client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None); }
    }
}