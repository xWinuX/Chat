using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Networking.Packets;

namespace Networking
{
    public abstract class TcpClient : SocketBehaviour
    {
        protected TcpClient(string address, int port) : base(address, port) { }

        private Socket _client;
        private bool   _closed;

        /// <summary>
        /// Starts the client
        /// </summary>
        public async Task Start()
        {
            IPAddress  ipAddress = IPAddress.Parse(Address);
            IPEndPoint endpoint  = new IPEndPoint(ipAddress, Port);

            _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await _client.ConnectAsync(endpoint);
                await SendAcceptPacket();

                ReceiveHandler(_client);
            }
            catch (Exception)
            {
                Close(_client);
                throw;
            }
        }

        /// <summary>
        /// Gracefully closes the socket by sending a closing packet to the server
        /// </summary>
        public async Task Close()
        {
            _closed = true;
            await SendClosingPacket();
            Close(_client);
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

                    ResolvePacket(data, numBytesRead);
                }
            }
            catch (Exception)
            {
                if (!_closed) { OnReceiveFail(); }
            }
        }

        protected async Task Send(Packet packet)
        {
            if (_client == null) { return; }

            byte[] bytes = packet.GetBytes();

            await _client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None);
        }

        protected abstract void ResolvePacket(byte[] data, int numBytesRead);

        protected abstract Task SendClosingPacket();

        protected abstract Task SendAcceptPacket();

        protected abstract void OnReceiveFail();
    }
}