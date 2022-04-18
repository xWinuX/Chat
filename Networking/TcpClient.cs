using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Networking.Packets;

namespace Networking
{
    public abstract class TcpClient
    {
        protected TcpClient(string address, int port)
        {
            _address            = address;
            _port               = port;
            _receiveCancelToken = _receiveCancelTokenSource.Token;
        }

        private readonly string _address;
        private readonly int    _port;

        private Socket _client;

        private readonly CancellationTokenSource _receiveCancelTokenSource = new CancellationTokenSource();
        private readonly CancellationToken       _receiveCancelToken;

        /// <summary>
        /// Starts the client
        /// </summary>
        public async Task Start()
        {
            IPAddress  ipAddress = IPAddress.Parse(_address);
            IPEndPoint endpoint  = new IPEndPoint(ipAddress, _port);

            _client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await _client.ConnectAsync(endpoint);
                await SendAcceptPacket();
                
                Task.Run(() => ReceiveHandler(_client), _receiveCancelToken);
            }
            catch (Exception)
            {
                ForceClose();
                throw;
            }
        }

        /// <summary>
        /// Forces socket to close without sending a closing packet
        /// use this when the connection to the server fails
        /// </summary>
        private void ForceClose()
        {
            try
            {
                _client.Shutdown(SocketShutdown.Both); 
                _receiveCancelTokenSource.Dispose();
            }
            finally { _client.Close(); }
        }

        /// <summary>
        /// Gracefully closes the socket by sending a closing packet to the server
        /// </summary>
        public async Task Close()
        {
            await SendClosingPacket();
            _receiveCancelTokenSource.Cancel();
            _receiveCancelTokenSource.Dispose();
            ForceClose();
        }

        private async Task ReceiveHandler(Socket client)
        {
            try
            {
                while (!_receiveCancelToken.IsCancellationRequested)
                {
                    byte[]             bytes        = new byte[Packet.BufferSize];
                    ArraySegment<byte> arraySegment = new ArraySegment<byte>(bytes);
                    int                numBytesRead = await client.ReceiveAsync(arraySegment, 0);
                    byte[]             data         = arraySegment.ToArray();

                    ResolvePacket(data, numBytesRead);
                }
            }
            catch (Exception e) { OnReceiveFail(); }
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