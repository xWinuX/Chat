using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Chat.Core;
using Chat.Utility;

namespace Chat.Windows
{
    public partial class Client : IChat
    {
        private readonly string    _userName;
        private readonly TcpClient _client;

        public Client(string userName)
        {
            _userName = userName;
            
            InitializeComponent();

            try
            {
                _client = new TcpClient("127.0.0.1", 11000, _userName, this);
                _client.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void BtnLeave_OnClick(object sender, RoutedEventArgs e) { }

        private void BtnSend_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxbMessage.Text == string.Empty) { return; }

            _client.SendMessage(_userName, TxbMessage.Text);

            TxbMessage.Text = string.Empty;
        }
        
        public void AddMessage(string userName, string message)
        {
            TextBlock textBlock = new TextBlockBuilder().WithMessage($"[{userName}]: {message}").WithColor(Colors.Black).Build();
            ScrvChat.AddText(textBlock);
        }

        public void AddServerMessage(string message)
        {
            TextBlock textBlock = new TextBlockBuilder().WithMessage(message).WithColor(Colors.Blue).Build();
            ScrvChat.AddText(textBlock);
        }
    }
}