using System.Threading.Tasks;
using ChatNetworking.Packets;
using ChatNetworking.Utility;
using Networking;
using Networking.Packets;

namespace ChatNetworking.Core
{
    public class ChatTcpClient : TcpClient
    {
        private readonly IChat  _chat;
        private readonly string _userName;

        public ChatTcpClient(string address, int port, string userName, IChat chat) : base(address, port)
        {
            _userName = userName;
            _chat     = chat;
        }

        /// <summary>
        /// Sends given message to server
        /// </summary>
        /// <param name="userName">Username that sends this message</param>
        /// <param name="message">Message to send</param>
        public async Task SendMessage(string userName, string message) { await Send(new ClientMessageSentPacket(userName, message)); }

        protected override void ResolvePacket(byte[] data, int numBytesRead)
        {
            PacketType packetType = Packet.GetType(data);

            if (packetType == PacketType.Invalid) { return; }

            switch (packetType)
            {
                case PacketType.ClientConnected:
                    ClientConnectedPacket clientConnectedPacket = PacketUtility.TryParse<ClientConnectedPacket>(data, numBytesRead);
                    _chat.AddServerMessage($"{clientConnectedPacket.UserName} has joined the server!");
                    break;
                case PacketType.ClientDisconnected:
                    ClientDisconnectedPacket clientDisconnectedPacket = PacketUtility.TryParse<ClientDisconnectedPacket>(data, numBytesRead);
                    _chat.AddServerMessage($"{clientDisconnectedPacket.UserName} has left the server!");
                    break;
                case PacketType.ClientMessageSent:
                    ClientMessageSentPacket clientMessageSentPacketPacket = PacketUtility.TryParse<ClientMessageSentPacket>(data, numBytesRead);
                    _chat.AddMessage(clientMessageSentPacketPacket.UserName, clientMessageSentPacketPacket.Message);
                    break;
            }
        }

        protected override async Task SendAcceptPacket() => await Send(new ClientConnectedPacket(_userName));

        protected override async Task SendClosingPacket() => await Send(new ClientDisconnectedPacket(_userName));

        protected override void OnReceiveFail() => _chat.ServerClosed();
    }
}