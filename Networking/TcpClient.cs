using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networking.Packets;

namespace Networking
{
    public class TcpClient
    {
        public TcpClient(string address, int port)
        {
            _address = address;
            _port    = port;
        }

        private readonly string _address;
        private readonly int    _port;

        private Socket _client;

        public async Task Start()
        {
            try
            {
                IPAddress  ipAddress = IPAddress.Parse(_address);
                IPEndPoint endpoint  = new IPEndPoint(ipAddress, _port);

                _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                await _client.ConnectAsync(endpoint);
                await SendAcceptPacket();

                await ReceiveHandler(_client);
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        public async void Close() { await SendClosingPacket(); }

        private void CloseSocket()
        {
            _client.Shutdown(SocketShutdown.Both);
            _client.Close();
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

                    bool stayConnected = ResolvePacket(data, numBytesRead);

                    if (!stayConnected) { break; }
                }
                
                CloseSocket();
            }
            catch (Exception e) { Console.WriteLine(e.ToString()); }
        }

        protected virtual bool ResolvePacket(byte[] data, int numBytesRead) => false;

        protected virtual async Task SendClosingPacket() { }

        protected virtual async Task SendAcceptPacket() { }

        protected async Task Send(Packet packet)
        {
            if (_client == null) { return; }

            byte[] bytes = packet.GetBytes();

            await _client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None);
        }
    }
}