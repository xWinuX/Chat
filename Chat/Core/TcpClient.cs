using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
            _chat   = chat;
        }

        public bool Connected { get; private set; }

        private readonly string _userName;
        private readonly string _address;
        private readonly int    _port;

        private IChat _chat;

        private Socket _client;

        public async Task Start()
        {
            try
            {
                IPAddress  ipAddress = IPAddress.Parse(_address);
                IPEndPoint endpoint  = new IPEndPoint(ipAddress, _port);

                _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                await _client.ConnectAsync(endpoint);
                _chat.AddMessage("DEBUG","Successfully connected");
                await Send(Packet.CreateClientConnectedPacket(_userName));
                _chat.AddMessage("DEBUG", "Sent connected packet");
                
                Connected = true;

                while (true) { await Receive(_client); }
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }
        
        private void Close(Socket client)
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }


        private async Task Receive(Socket client)
        {
            try
            {
                Packet             state        = new Packet();
                ArraySegment<byte> arraySegment = new ArraySegment<byte>(state.Buffer, 0, Packet.BufferSize);
                int                numBytesRead = await client.ReceiveAsync(arraySegment, 0);

                byte[] data = arraySegment.ToArray();
                
                PacketState   packetState = (PacketState)Enum.Parse(typeof(PacketState), data[0].ToString());
                StringBuilder sb          = new StringBuilder();
                sb.Append(Encoding.Default.GetString(state.Buffer, 1, numBytesRead - 1));

                _chat.AddMessage("Test",sb.ToString());
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        public async Task Send(string data) { await Send(Packet.CreateClientConnectedPacket(data)); }

        private async Task Send(Packet packet)
        {
            if (_client == null) { return; }

            await _client.SendAsync(new ArraySegment<byte>(packet.Buffer, 0, Packet.BufferSize), SocketFlags.None);
        }
    }
}