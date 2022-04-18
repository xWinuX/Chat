using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Chat.Utility;
using ChatNetworking.Core;

namespace Chat.Windows
{
    public partial class ClientWindow : IChat
    {
        private readonly string _userName;
        private readonly string _address;
        private readonly int    _port;

        private readonly ChatTcpClient _client;

        private bool _clientSuccessfullyStarted = false;

        public ClientWindow(string userName, string address, int port)
        {
            _userName = userName;
            _address  = address;
            _port     = port;

            InitializeComponent();

            _client = new ChatTcpClient(_address, _port, _userName, this);
        }

        /// <summary>
        /// Adds Message to chat panel
        /// </summary>
        /// <param name="userName">Which user wrote the message</param>
        /// <param name="message">Message to display</param>
        public void AddMessage(string userName, string message)
        {
            TextBlock textBlock = new TextBlockBuilder().WithMessage($"[{userName}]: {message}").WithColor(Colors.Black).Build();
            ScrvChat.AddText(textBlock);
        }

        /// <summary>
        /// Adds server message (message without user and different color) to chat panel 
        /// </summary>
        /// <param name="message">Message to show</param>
        public void AddServerMessage(string message)
        {
            TextBlock textBlock = new TextBlockBuilder().WithMessage(message).WithColor(Colors.Blue).Build();
            ScrvChat.AddText(textBlock);
        }

        public void ServerClosed()
        {
            WindowUtility.ShowErrorMessageBox(this, "Connection to server failed or was closed");
            WindowUtility.OpenNewWindowAndCloseCurrentOne<LoginWindow>(this);
        }

        private async void BtnLeave_OnClick(object sender, RoutedEventArgs e)
        {
            await _client.Close();
            Close();
        }

        private async void BtnSend_OnClick(object sender, RoutedEventArgs e)
        {
            if (TxbMessage.Text == string.Empty) { return; } // Message shouldn't be empty 

            await _client.SendMessage(_userName, TxbMessage.Text);

            TxbMessage.Text = string.Empty;
        }

        private async void ClientWindow_OnClosed(object? sender, EventArgs e)
        {
            if (!_clientSuccessfullyStarted) { return; }

            await _client.Close();
        }

        private async void ClientWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            // Try to connected to server
            AddServerMessage("Trying to connect to server...");
            try
            {
                await _client.Start();
                _clientSuccessfullyStarted = true;
                AddServerMessage("Successfully connected to server!");
            }
            catch (Exception) { ServerClosed(); } // If connection fails display message box and go back to login
        }
    }
}