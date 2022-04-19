using System.Threading.Tasks;
using ChatNetworking.Packets;
using ChatNetworking.Utility;
using Networking;
using Networking.Packets;

namespace ChatNetworking.Core
{
    public class ChatTcpClient : TcpClient
    {
        private readonly string _userName;
        private readonly IChat  _chat;

        public ChatTcpClient(string address, int port, string userName, IChat chat) : base(address, port)
        {
            _userName = userName;
            _chat     = chat;
        }

        public async Task SendMessage(string userName, string message) { await Send(new ClientMessageSentPacket(userName, message)); }

        protected override void ResolvePacket(byte[] data, int numBytesRead)
        {
            PacketType packetType = Packet.GetType(data);

            if (packetType == PacketType.Invalid) { return; }

            switch (packetType)
            {
                case PacketType.ClientConnected:
                    ClientConnectedPacket clientConnectedPacket = PacketUtility.TryParse<ClientConnectedPacket>(data, numBytesRead);
                    _chat.AddServerMessage($"{clientConnectedPacket.UserName} has joined the Server!");
                    break;
                case PacketType.ClientDisconnected:
                    ClientDisconnectedPacket clientDisconnectedPacket = PacketUtility.TryParse<ClientDisconnectedPacket>(data, numBytesRead);
                    _chat.AddServerMessage($"{clientDisconnectedPacket.UserName} has left the Server!");
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