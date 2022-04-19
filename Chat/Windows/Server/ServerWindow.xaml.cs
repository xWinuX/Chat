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
        private static   TextBlockBuilder Builder => new TextBlockBuilder();
        private readonly string           _address;
        private readonly int              _port;

        private ChatTcpServer _server;

        public ServerWindow(string address, int port)
        {
            _address = address;
            _port    = port;
            InitializeComponent();
        }

        public void Log(string message) { AppendToConsole(Builder.WithMessage(message).Build()); }
        public void LogSuccess(string message) { AppendToConsole(Builder.WithMessage(message).WithColor(Colors.Lime).Build()); }
        public void LogWarning(string message) { AppendToConsole(Builder.WithMessage(message).WithColor(Colors.Yellow).Build()); }
        public void LogError(string message) { AppendToConsole(Builder.WithMessage(message).WithColor(Colors.Red).Build()); }

        /// <summary>
        /// Adds timestamp to the front of the given text block
        /// </summary>
        /// <param name="textBlock">Text block to add timestamp to</param>
        private static void AddTimeToTextBlock(TextBlock textBlock)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            textBlock.Text = "[" + time + "]" + " " + textBlock.Text;
        }

        /// <summary>
        /// Appends the given text block to the console
        /// </summary>
        /// <param name="textBlock">Text block to append</param>
        private void AppendToConsole(TextBlock textBlock)
        {
            AddTimeToTextBlock(textBlock);
            ScrtConsole.AddText(textBlock);
        }

        private void ServerWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _server = new ChatTcpServer(_address, _port, this);
                _server.Run();
            }
            catch (Exception)
            {
                WindowUtility.ShowErrorMessageBox(this, "Failed to start server!");
                WindowUtility.OpenNewWindowAndCloseCurrentOne<ServerSetupWindow>(this);
            }

        }
    }
}