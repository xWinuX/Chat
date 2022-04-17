using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chat.Packets;

namespace Chat.Core
{
    public class TcpServer
    {
        private readonly List<Socket> _clients = new List<Socket>();

        private readonly IConsole _console;

        private Socket _listener;

        public TcpServer(IConsole console) { _console = console; }

        public void Run()
        {
            IPAddress  ipAddress     = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                _console.LogSuccess("Server Successfully started");
                _console.Log("Waiting for connection");

                AcceptHandler();
            }
            catch (Exception e)
            {
                _console.LogError("SocketException : " + e);
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
            try
            {
                while (true)
                {
                    byte[]             bytes        = new byte[Packet.BufferSize];
                    ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes);
                    int                numBytesRead = await client.ReceiveAsync(bytes, SocketFlags.None);
                    byte[]             data         = arraySegment.ToArray();

                    PacketType packetType = Packet.GetType(data);
                    if (packetType != PacketType.Invalid)
                    {
                        try 
                        {
                            switch (packetType)
                            {
                                case PacketType.ClientConnected:
                                    ClientConnectedPacket clientConnectedPacket = Packet.TryParse<ClientConnectedPacket>(data, numBytesRead);
                                    _console.LogSuccess($"User {clientConnectedPacket.UserName} joined!");
                                    await SendToConnectedClients(clientConnectedPacket.GetBytes());
                                    break;
                                case PacketType.ClientMessageSent:
                                    ClientMessageSentPacket clientMessageSentPacket = Packet.TryParse<ClientMessageSentPacket>(data, numBytesRead);
                                    _console.Log($"User: {clientMessageSentPacket.UserName} has sent the following: {clientMessageSentPacket.Message}");
                                    await SendToConnectedClients(clientMessageSentPacket.GetBytes());
                                    break;
                            }
                        }
                        catch (Exception e)
                        {
                            _console.LogError("An Error occured while trying to parse a packet!");
                            _console.LogError(e.ToString());
                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {
                _console.LogWarning("Client has forcefully disconnected!");
                CloseHandler(client);
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
            catch (Exception) { _console.LogWarning("Failed to close the clients handler!"); }
        }

        private async Task SendToConnectedClients(byte[] bytes)
        {
            foreach (Socket client in _clients) { await Send(client, bytes); }
        }

        private async Task Send(Socket client, byte[] bytes) { await client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None); }
    }
}