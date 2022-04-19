using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Chat.Utility;
using ChatNetworking.Core;
using Networking;

namespace Chat.Windows.Server
{
    public partial class ServerWindow : IConsole
    {
        private readonly string _address;
        private readonly int    _port;

        private ChatTcpServer _server;
        
        public ServerWindow(string address, int port)
        {
            _address = address;
            _port    = port;
            InitializeComponent();
        }
        
        private static TextBlockBuilder Builder => new TextBlockBuilder();

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

        private void ServerWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _server = new ChatTcpServer(_address, _port, this);
                _server.Run();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }

        }
    }
}