using System.Net.Sockets;
using System.Threading.Tasks;
using ChatNetworking.Packets;
using ChatNetworking.Utility;
using Networking;
using Networking.Packets;

namespace ChatNetworking.Core
{
    public class ChatTcpServer : TcpServer
    {
        public ChatTcpServer(string address, int port, IConsole console) : base(address, port, console) { }

        protected override async Task<bool> ResolvePacket(Socket client, byte[] data, int numBytesRead)
        {
            PacketType packetType = Packet.GetType(data);
            if (packetType != PacketType.Invalid)
            {
                switch (packetType)
                {
                    case PacketType.ClientConnected:
                        ClientConnectedPacket clientConnectedPacket = PacketUtility.TryParse<ClientConnectedPacket>(data, numBytesRead);
                        Console.LogSuccess($"User {clientConnectedPacket.UserName} joined!");
                        await SendToConnectedClients(clientConnectedPacket.GetBytes());
                        break;
                    case PacketType.ClientDisconnected:
                        ClientDisconnectedPacket clientDisconnectedPacket = PacketUtility.TryParse<ClientDisconnectedPacket>(data, numBytesRead);
                        Console.LogSuccess($"User {clientDisconnectedPacket.UserName} disconnected!");
                        CloseClient(client);
                        await SendToConnectedClients(clientDisconnectedPacket.GetBytes());
                        return false;
                    case PacketType.ClientMessageSent:
                        ClientMessageSentPacket clientMessageSentPacket = PacketUtility.TryParse<ClientMessageSentPacket>(data, numBytesRead);
                        Console.Log($"User: {clientMessageSentPacket.UserName} has sent the following: {clientMessageSentPacket.Message}");
                        await SendToConnectedClients(clientMessageSentPacket.GetBytes());
                        break;
                }
            }

            return true;
        }
    }
}