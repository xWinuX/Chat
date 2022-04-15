using System.ComponentModel;
using Chat.Logging;

namespace Chat.Utility
{
    public static class LogUtility
    {
        public static void Log(BackgroundWorker backgroundWorker, string message)
        {
            backgroundWorker?.ReportProgress(0, new LoggingBackgroundWorkerProgressData(message, LogState.Normal));
        }

        public static void LogSuccess(BackgroundWorker backgroundWorker, string message)
        {
            backgroundWorker?.ReportProgress(0, new LoggingBackgroundWorkerProgressData(message, LogState.Success));
        }

        public static void LogWarning(BackgroundWorker backgroundWorker, string message)
        {
            backgroundWorker?.ReportProgress(0, new LoggingBackgroundWorkerProgressData(message, LogState.Warning));
        }

        public static void LogError(BackgroundWorker backgroundWorker, string message)
        {
            backgroundWorker?.ReportProgress(0, new LoggingBackgroundWorkerProgressData(message, LogState.Error));
        }
    }
}