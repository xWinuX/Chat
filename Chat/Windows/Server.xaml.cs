﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Chat.Core;
using Chat.Utility;

namespace Chat.Windows
{
    public partial class Server : IConsole
    {
        public Server() { InitializeComponent(); }

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
                    TcpServer server = new TcpServer(this);
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

        private TextBlockBuilder _builder => new TextBlockBuilder();

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

        public void Log(string message) { AppendToConsole(_builder.WithMessage(message).Build()); }

        public void LogSuccess(string message) { AppendToConsole(_builder.WithMessage(message).WithColor(Colors.Lime).Build()); }

        public void LogWarning(string message) { AppendToConsole(_builder.WithMessage(message).WithColor(Colors.Yellow).Build()); }

        public void LogError(string message) { AppendToConsole(_builder.WithMessage(message).WithColor(Colors.Red).Build()); }
    }
}