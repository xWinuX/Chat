using System;

namespace Networking.Packets
{
    public abstract class Packet
    {
        public const    int BufferSize                    = 1024;
        protected const int EncodingWorstCasePerCharacter = 4;

        protected abstract PacketType Type { get; }

        public abstract byte[] GetBytes();

        public static PacketType GetType(byte[] bytes)
        {
            try { return (PacketType)Enum.Parse(typeof(PacketType), bytes[0].ToString()); }
            catch (Exception) { return PacketType.Invalid; }
        }
    }
}