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
    }
}