﻿using System.Net.Sockets;

namespace Networking
{
    public abstract class SocketBehaviour
    {
        protected readonly string Address;
        protected readonly int    Port;

        protected SocketBehaviour(string address, int port)
        {
            Address = address;
            Port    = port;
        }

        protected static void Close(Socket socket)
        {
            try { socket.Shutdown(SocketShutdown.Both); }
            finally { socket.Close(); }
        }
    }
}