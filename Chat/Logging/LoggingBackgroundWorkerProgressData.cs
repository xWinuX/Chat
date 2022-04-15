namespace Chat.Logging
{
    public struct LoggingBackgroundWorkerProgressData
    {
        public string   Message  { get; }
        public LogState LogState { get; }

        public LoggingBackgroundWorkerProgressData(string message, LogState logState)
        {
            Message  = message;
            LogState = logState;
        }
    }
}