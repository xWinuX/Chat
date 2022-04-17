using ChatNetworking.Core;

namespace LinuxServer.Core
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ChatTcpServer server = new ChatTcpServer(new Logger());
            server.Run();
        }
    }
}