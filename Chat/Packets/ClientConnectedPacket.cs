namespace Chat.Packets
{
    public class ClientConnectedPacket : SimplePacket
    {
        public override PacketType Type => PacketType.ClientConnected;

        public string UserName { get; }

        public ClientConnectedPacket(string userName) { UserName = userName; }

        public override byte[] GetBytes() => ConstructPacket(UserName);
    }
}