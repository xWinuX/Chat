using System;

namespace Networking.Packets
{
    public abstract class Packet
    {
        public const    int BufferSize                    = 1024;
        protected const int EncodingWorstCasePerCharacter = 4;

        protected abstract PacketType Type { get; }

        /// <summary>
        /// Constructs byte array out of the package
        /// </summary>
        /// <returns>Byte array that can be send over network</returns>
        public abstract byte[] GetBytes();

        /// <summary>
        /// Gets the type of the packet by looking at the first byte
        /// </summary>
        /// <param name="bytes">Byte array to get the type from</param>
        /// <returns>The packet type, PacketType.Invalid will be returned if the parsing fails</returns>
        public static PacketType GetType(byte[] bytes)
        {
            try { return (PacketType)Enum.Parse(typeof(PacketType), bytes[0].ToString()); }
            catch (Exception) { return PacketType.Invalid; }
        }
    }
}