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

        public void Run()
        {
            IPAddress  ipAddress     = IPAddress.Parse("192.168.1.127");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 9999);
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
                Console.LogError("SocketException : " + e);
                throw;
            }
        }

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

        protected virtual async Task<bool> ResolvePacket(Socket client, byte[] data, int numBytesRead) => false;

        protected void CloseClient(Socket client)
        {
            Console.Log("Close Client");
            
            if (client == null) { return; }

            Console.Log(_clients.Count.ToString());
            _clients.Remove(client);

            Console.Log(_clients.Count.ToString());
            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Console.Log("Finished closing");
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