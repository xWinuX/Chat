using Networking.Packets;

namespace ChatNetworking.Packets
{
    public class ClientConnectedPacket : SimplePacket
    {
        protected override PacketType Type => PacketType.ClientConnected;

        public string UserName { get; }
        public ClientConnectedPacket(string userName) { UserName = userName; }

        public override byte[] GetBytes() => ConstructPacket(UserName);
    }
}