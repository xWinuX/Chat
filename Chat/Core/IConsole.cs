namespace Chat.Core
{
    public interface IConsole
    {
        void Log(string message);
        void LogSuccess(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}