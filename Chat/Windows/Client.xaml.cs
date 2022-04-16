using System;
using System.Windows;
using System.Windows.Controls;
using Chat.Core;
using Chat.Utility;

namespace Chat.Windows
{
    public interface IChat
    {
        void AddMessage(string userName, string message);
    }
    
    public partial class Client : Window, IChat
    {
        private TcpClient _client;

        public Client()
        {
            InitializeComponent();

            try
            {
                _client = new TcpClient("127.0.0.1", 11000, "TestUser", this);
                _client.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void BtnLeave_OnClick(object sender, RoutedEventArgs e) { }

        private void BtnSend_OnClick(object sender, RoutedEventArgs e) { _client.Send("AHSdjashhjhAGHHHH"); }
        
        public void AddMessage(string userName, string message)
        {
            TextBlock textBlock = new TextBlockBuilder().WithMessage($"[{userName}]: {message}").Build();
            ScrvChat.AddText(textBlock);
        }
    }
}