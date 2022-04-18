using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Chat.Utility;
using ChatNetworking.Core;

namespace Chat.Windows
{
    public partial class Client : IChat
    {
        private readonly string _userName;
        private readonly string _address;
        private readonly int    _port;

        private readonly ChatTcpClient _client;

        public Client(string userName, string address, int port)
        {
            _userName = userName;
            _address  = address;
            _port     = port;

            InitializeComponent();

            _client = new ChatTcpClient("85.7.3.2", 9999, _userName, this);
        }

        protected override void OnInitialized(EventArgs e)
        {
            try { _client.Start(); }
            catch (Exception)
            {
                WindowUtility.ShowErrorMessageBox(this, "Failed to connect to server!");
                WindowUtility.OpenNewWindowAndCloseCurrentOne<Login>(this);
            }
        }

        private async void BtnLeave_OnClick(object sender, RoutedEventArgs e)
        {
            await _client.Close();
            Close();
        }

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

        private async void Client_OnClosed(object? sender, EventArgs e) { await _client.Close(); }
    }
}