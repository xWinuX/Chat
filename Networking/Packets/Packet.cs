using System;
using System.Text;

namespace Networking.Packets
{
    public abstract class Packet
    {
        public const int BufferSize                    = 1024;
        public const int EncodingWorstCasePerCharacter = 4;

        public abstract PacketType Type { get; }

        public abstract byte[] GetBytes();

        public static PacketType GetType(byte[] bytes)
        {
            try { return (PacketType)Enum.Parse(typeof(PacketType), bytes[0].ToString()); }
            catch (Exception) { return PacketType.Invalid; }
        }

        public static T TryParse<T>(byte[] bytes, int bytesSent = BufferSize) where T : Packet
        {
            try
            {
                PacketType type = (PacketType)Enum.Parse(typeof(PacketType), bytes[0].ToString());

                string userName;
                switch (type)
                {
                    case PacketType.ClientConnected:
                        userName = Encoding.UTF8.GetString(bytes, 1, bytesSent - 1);
                        return (new ClientConnectedPacket(userName)) as T;
                    case PacketType.ClientMessageSent:
                        int userNameLength = bytes[1];
                        userName = Encoding.UTF8.GetString(bytes, 2, userNameLength);
                        string message = Encoding.UTF8.GetString(bytes, ClientMessageSentPacket.MessageStartIndex, bytesSent - ClientMessageSentPacket.MessageStartIndex);
                        return (new ClientMessageSentPacket(userName, message)) as T;
                }

                return null;
            }
            catch (Exception e) { return null; }
        }
    }
}