using Networking.Packets;

namespace ChatNetworking.Packets
{
    public class ClientDisconnectedPacket : SimplePacket
    {
        public override PacketType Type => PacketType.ClientDisconnected;

        public string UserName { get; }
        public ClientDisconnectedPacket(string userName) { UserName = userName; }

        public override byte[] GetBytes() => ConstructPacket(UserName);
    }
}