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
        private Socket _client;
        private bool   _closed;
        protected TcpClient(string address, int port) : base(address, port) { }

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

                ReceiveHandler();
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

        /// <summary>
        /// Sends given packet to server
        /// </summary>
        /// <param name="packet">Packet to send</param>
        protected async Task Send(Packet packet)
        {
            if (_client == null) { return; }

            byte[] bytes = packet.GetBytes();

            await _client.SendAsync(new ArraySegment<byte>(bytes, 0, bytes.Length), SocketFlags.None);
        }

        /// <summary>
        /// Resolves given byte array and
        /// </summary>
        /// <param name="data">Byte array to resolve</param>
        /// <param name="numBytesRead">Number of bytes read</param>
        protected abstract void ResolvePacket(byte[] data, int numBytesRead);

        /// <summary>
        /// Sends the closing packet, it signals the server that this client is about to disconnect
        /// </summary>
        /// <returns>Task that resolves into void</returns>
        protected abstract Task SendClosingPacket();

        /// <summary>
        /// Sends the accept packet
        /// </summary>
        /// <returns></returns>
        protected abstract Task SendAcceptPacket();

        /// <summary>
        /// This will happen if the receive fails (server probably unreachable)
        /// </summary>
        protected abstract void OnReceiveFail();

        /// <summary>
        /// Handles receiving data from server
        /// </summary>
        private async Task ReceiveHandler()
        {
            try
            {
                while (true)
                {
                    byte[]             bytes        = new byte[Packet.BufferSize];
                    ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes);
                    int                numBytesRead = await _client.ReceiveAsync(arraySegment, 0);
                    byte[]             data         = arraySegment.ToArray();

                    ResolvePacket(data, numBytesRead);
                }
            }
            catch (Exception)
            {
                if (!_closed) { OnReceiveFail(); }
            }
        }
    }
}