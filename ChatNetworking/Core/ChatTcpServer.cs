using System.Threading.Tasks;
using Networking;
using Networking.Packets;

namespace ChatNetworking.Core
{
    public class ChatTcpServer : TcpServer
    {
        public ChatTcpServer(IConsole console) : base(console) { }

        protected override async Task ResolvePacket(byte[] data, int numBytesRead)
        {
            
            PacketType packetType = Packet.GetType(data);
            if (packetType != PacketType.Invalid)
            {
                switch (packetType)
                {
                    case PacketType.ClientConnected:
                        ClientConnectedPacket clientConnectedPacket = Packet.TryParse<ClientConnectedPacket>(data, numBytesRead);
                        Console.LogSuccess($"User {clientConnectedPacket.UserName} joined!");
                        await SendToConnectedClients(clientConnectedPacket.GetBytes());
                        break;
                    case PacketType.ClientMessageSent:
                        ClientMessageSentPacket clientMessageSentPacket = Packet.TryParse<ClientMessageSentPacket>(data, numBytesRead);
                        Console.Log($"User: {clientMessageSentPacket.UserName} has sent the following: {clientMessageSentPacket.Message}");
                        await SendToConnectedClients(clientMessageSentPacket.GetBytes());
                        break;
                }
            }
        }
    }
}