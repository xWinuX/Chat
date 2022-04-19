using System.Text;
using Networking.Packets;

namespace ChatNetworking.Packets
{
    public class ClientMessageSentPacket : Packet
    {
        public string Message  { get; }

        public static   int MessageStartIndex => (2 + (EncodingWorstCasePerCharacter * 15)); // State(1) + UsernameLength(1) + Username(15 character => 4 bytes per character UTF8)
        public override PacketType Type => PacketType.ClientMessageSent;

        public string UserName { get; }

        public ClientMessageSentPacket(string username, string message)
        {
            UserName = username.Length > 15 ? username.Substring(0, 15) : username;
            Message  = message;
        }

        public override byte[] GetBytes()
        {
            byte[] userNameBytes = Encoding.UTF8.GetBytes(UserName);
            byte[] messageBytes  = Encoding.UTF8.GetBytes(Message);
            byte[] bytes         = new byte[2 + (15 * EncodingWorstCasePerCharacter) + messageBytes.Length];
            bytes[0] = (byte)Type;
            bytes[1] = (byte)userNameBytes.Length;

            for (int i = 0; i < userNameBytes.Length; i++) { bytes[2 + i] = userNameBytes[i]; }

            for (int i = 0; i < messageBytes.Length; i++) { bytes[MessageStartIndex + i] = messageBytes[i]; }

            return bytes;
        }
    }
}