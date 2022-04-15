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
    public partial class Server : IConsole
    {
        public Server() { InitializeComponent(); }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += (sender, _) =>
            {
                try
                {
                    BackgroundWorker backgroundWorker = (BackgroundWorker)sender;
                    TcpServer        server           = new TcpServer(backgroundWorker);
                
                    server.Run();
                }
                catch (Exception exception)
                {
                    // ignored
                }
            };
            worker.ProgressChanged += (sender, args) =>
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
            }; 
            
            worker.RunWorkerAsync();
        }

        private ServerConsoleEntryBuilder _builder => new ServerConsoleEntryBuilder();

        private void BtnCloseServer_OnClick(object sender, RoutedEventArgs e)
        {
            Log("Default");
            LogSuccess("Success");
            LogWarning("Warning");
            LogError("Error");
        }

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