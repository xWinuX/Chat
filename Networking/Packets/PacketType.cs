namespace Networking.Packets
{
    public enum PacketType : byte
    {
        Invalid,
        ClientConnected,
        ClientDisconnected,
        ClientMessageSent
    }
}