using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Chat.Core;
using Chat.Logging;
using Chat.Utility;

namespace Chat.Windows
{
    public partial class Server
    {
        public Server() { InitializeComponent(); }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress =  true;
            worker.DoWork                += StartTcpServer;

            worker.ProgressChanged += OnTCPServerProgressChanged;

            worker.RunWorkerAsync();
        }

        private static void StartTcpServer(object sender, DoWorkEventArgs _)
        {
            void OpenServer()
            {
                try
                {
                    BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
                    TcpServer        server           = new TcpServer(backgroundWorker);

                    server.Run();
                }
                catch (Exception)
                {
                    BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
                    LogUtility.LogError(backgroundWorker, "TCP Server crashed, restarting...");
                    OpenServer();
                }
            }

            OpenServer();
        }

        private void OnTCPServerProgressChanged(object sender, ProgressChangedEventArgs args)
        {
            if (args.UserState == null) { return; }

            LoggingBackgroundWorkerProgressData loggingBackgroundWorkerProgressData = (LoggingBackgroundWorkerProgressData)args.UserState;

            switch (loggingBackgroundWorkerProgressData.LogState)
            {
                case LogState.Normal:
                    Log(loggingBackgroundWorkerProgressData.Message);
                    break;
                case LogState.Success:
                    LogSuccess(loggingBackgroundWorkerProgressData.Message);
                    break;
                case LogState.Warning:
                    LogWarning(loggingBackgroundWorkerProgressData.Message);
                    break;
                case LogState.Error:
                    LogError(loggingBackgroundWorkerProgressData.Message);
                    break;
            }
        }

        private ServerConsoleEntryBuilder _builder => new ServerConsoleEntryBuilder();

        private void BtnCloseServer_OnClick(object sender, RoutedEventArgs e) { }

        private static void AddTimeToTextBlock(TextBlock textBlock)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            textBlock.Text = "[" + time + "]" + " " + textBlock.Text;
        }

        private void AppendToConsole(TextBlock textBlock)
        {
            AddTimeToTextBlock(textBlock);
            Console.Children.Add(textBlock);
            ScrollViewer.ScrollToBottom();
        }

        public void Log(string message) { AppendToConsole(_builder.WithMessage(message).Build()); }

        public void LogSuccess(string message) { AppendToConsole(_builder.WithMessage(message).WithColor(Colors.Lime).Build()); }

        public void LogWarning(string message) { AppendToConsole(_builder.WithMessage(message).WithColor(Colors.Yellow).Build()); }

        public void LogError(string message) { AppendToConsole(_builder.WithMessage(message).WithColor(Colors.Red).Build()); }
    }
}