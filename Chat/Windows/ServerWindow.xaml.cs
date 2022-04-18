using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Chat.Utility;
using ChatNetworking.Core;
using Networking;

namespace Chat.Windows
{
    public partial class ServerWindow : IConsole
    {
        public ServerWindow() { InitializeComponent(); }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            StartTcpServer();
        }

        private void StartTcpServer()
        {
            void OpenServer()
            {
                try
                {
                    ChatTcpServer server = new ChatTcpServer(this);
                    server.Run();
                }
                catch (Exception)
                {
                    LogError("TCP Server crashed, restarting...");
                    OpenServer();
                }
            }

            OpenServer();
        }

        private static TextBlockBuilder Builder => new TextBlockBuilder();

        private void BtnCloseServer_OnClick(object sender, RoutedEventArgs e) { }

        private static void AddTimeToTextBlock(TextBlock textBlock)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            textBlock.Text = "[" + time + "]" + " " + textBlock.Text;
        }

        private void AppendToConsole(TextBlock textBlock)
        {
            AddTimeToTextBlock(textBlock);
            ScrtConsole.AddText(textBlock);
        }

        public void Log(string message) { AppendToConsole(Builder.WithMessage(message).Build()); }

        public void LogSuccess(string message) { AppendToConsole(Builder.WithMessage(message).WithColor(Colors.Lime).Build()); }

        public void LogWarning(string message) { AppendToConsole(Builder.WithMessage(message).WithColor(Colors.Yellow).Build()); }

        public void LogError(string message) { AppendToConsole(Builder.WithMessage(message).WithColor(Colors.Red).Build()); }
    }
}