using System.Text;

namespace Chat.Core
{
    public class Packet
    {
        public const int    BufferSize = 1024;
        public       byte[] Buffer { get; }

        public Packet() { Buffer = new byte[BufferSize]; }

        private static Packet GetPacketWithState(PacketState packetState)
        {
            Packet packet = new Packet { Buffer = { [0] = (byte)packetState } };
            return packet;
        }

        private void AppendString(string s)
        {
            byte[] bytes = Encoding.Default.GetBytes(s);
            for (int i = 0; i < bytes.Length; i++) { Buffer[1 + i] = bytes[i]; }
        }

        public static Packet CreateClientConnectedPacket(string userName)
        {
            Packet packet = GetPacketWithState(PacketState.ClientConnected);
            packet.AppendString(userName);

            return packet;
        }

        public PacketState GetState() { }
    }
}