using System;
using System.Text;
using ChatNetworking.Packets;
using Networking.Packets;

namespace ChatNetworking.Utility
{
    public static class PacketUtility
    {
        /// <summary>
        /// Tries to parse given byte array to a valid packet
        /// </summary>
        /// <param name="bytes">Byte array to parse</param>
        /// <param name="numBytesRead">Bytes read</param>
        /// <typeparam name="T">Packet to try to parse the bytes to</typeparam>
        /// <returns>Either the specified Packet or null if it failed</returns>
        public static T TryParse<T>(byte[] bytes, int numBytesRead = Packet.BufferSize) where T : Packet
        {
            try
            {
                PacketType type = (PacketType)Enum.Parse(typeof(PacketType), bytes[0].ToString());

                string userName;
                switch (type)
                {
                    case PacketType.ClientConnected:
                        userName = Encoding.UTF8.GetString(bytes, 1, numBytesRead - 1);
                        return new ClientConnectedPacket(userName) as T;
                    case PacketType.ClientDisconnected:
                        userName = Encoding.UTF8.GetString(bytes, 1, numBytesRead - 1);
                        return new ClientDisconnectedPacket(userName) as T;
                    case PacketType.ClientMessageSent:
                        int userNameLength = bytes[1];
                        userName = Encoding.UTF8.GetString(bytes, 2, userNameLength);
                        string message = Encoding.UTF8.GetString(bytes, ClientMessageSentPacket.MessageStartIndex, numBytesRead - ClientMessageSentPacket.MessageStartIndex);
                        return new ClientMessageSentPacket(userName, message) as T;
                }

                return null;
            }
            catch (Exception) { return null; }
        }
    }
}