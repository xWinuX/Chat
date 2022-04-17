using System;
using Networking;

namespace LinuxServer.Core
{
    public class Logger : IConsole
    {
        public void Log(string message) { Console.WriteLine(message); }
        public void LogSuccess(string message) { Console.WriteLine(message); }
        public void LogWarning(string message) { Console.WriteLine(message); }
        public void LogError(string message) { Console.WriteLine(message); }
    }
}