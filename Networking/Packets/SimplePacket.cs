using System.Text;

namespace Networking.Packets
{
    public abstract class SimplePacket : Packet
    {
        /// <summary>
        /// Constructs a simple byte array with a type at the first byte and a message on the last ones
        /// </summary>
        /// <param name="data">String to add to the payload</param>
        /// <returns></returns>
        protected byte[] ConstructPacket(string data)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] bytes     = new byte[dataBytes.Length + 1];

            bytes[0] = (byte)Type;

            for (int i = 0; i < dataBytes.Length; i++) { bytes[1 + i] = dataBytes[i]; }

            return bytes;
        }
    }
}