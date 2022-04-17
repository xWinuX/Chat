using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Chat.Packets;
using Chat.Windows;

namespace Chat.Core
{
    public class TcpClient
    {
        public TcpClient(string address, int port, string userName, IChat chat)
        {
            _address  = address;
            _port     = port;
            _userName = userName;
            _chat     = chat;
        }

        private readonly string _userName;
        private readonly string _address;
        private readonly int    _port;
        private readonly IChat  _chat;

        private Socket _client;

        private bool _socketIsSending;
        private bool _socketIsReceiving;

        public async Task Start()
        {
            try
            {
                IPAddress  ipAddress = IPAddress.Parse(_address);
                IPEndPoint endpoint  = new IPEndPoint(ipAddress, _port);

                _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                await _client.ConnectAsync(endpoint);
                await Send(new ClientConnectedPacket(_userName));
                
                ReceiveHandler(_client);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        private void Close(Socket client)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        
        private async Task ReceiveHandler(Socket client)
        {
            try
            {
                while (true)
                {
                    byte[]             bytes        = new byte[Packet.BufferSize];
                    ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes);
                    int                numBytesRead = await client.ReceiveAsync(arraySegment, 0);
                    byte[]             data         = arraySegment.ToArray();

                    PacketType packetType = Packet.GetType(data);

                    if (packetType != PacketType.Invalid)
                    {
                        switch (packetType)
                        {
                            case PacketType.ClientConnected:
                                ClientConnectedPacket clientConnectedPacket = Packet.TryParse<ClientConnectedPacket>(data, numBytesRead);
                                _chat.AddServerMessage($"{clientConnectedPacket.UserName} has joined the Server!");
                                break;
                            case PacketType.ClientMessageSent:
                                ClientMessageSentPacket clientMessageSentPacketPacket = Packet.TryParse<ClientMessageSentPacket>(data, numBytesRead);
                                _chat.AddMessage(clientMessageSentPacketPacket.UserName, clientMessageSentPacketPacket.Message);
                                break;
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        public async Task SendMessage(string userName, string message) { await Send(new ClientMessageSentPacket(userName, message)); }

        private async Task Send(Packet packet)
        {
            if (_client == null) { return; }
            
            byte[] bytes = packet.GetBytes();

            await _client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None);
        }
    }
}