using System.Threading.Tasks;
using Networking;
using Networking.Packets;

namespace ChatNetworking.Core
{
    public class ChatTcpClient : TcpClient
    {
        private readonly IChat _chat;

        public ChatTcpClient(string address, int port, string userName, IChat chat) : base(address, port, userName)
        {
            _chat = chat;
        }
        
        public async Task SendMessage(string userName, string message) { await Send(new ClientMessageSentPacket(userName, message)); }

        protected override void ResolvePacket(byte[] data, int numBytesRead)
        {
            PacketType packetType = Packet.GetType(data);

            if (packetType == PacketType.Invalid) { return; }

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