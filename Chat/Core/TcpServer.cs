using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Chat.Utility;

namespace Chat.Core
{
    public class TcpServer
    {
        private readonly List<Socket> _clients = new List<Socket>();
        private readonly List<string> _chatLog = new List<string>();

        private readonly ManualResetEvent _allDone = new ManualResetEvent(false);
        private readonly BackgroundWorker _backgroundWorker;

        private Socket _listener;

        public TcpServer(BackgroundWorker backgroundWorker) { _backgroundWorker = backgroundWorker; }

        public void Run()
        {
            IPAddress  ipAddress     = IPAddress.Parse("127.0.0.1");
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(100);

                while (true)
                {
                    _allDone.Reset();

                    LogUtility.Log(_backgroundWorker, "Waiting for connection");
                    _listener.BeginAccept(AcceptCallback, _listener);

                    _allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                LogUtility.LogError(_backgroundWorker, "SocketException : " + e);
                throw;
            }
        }

        private void BeginReceive(Socket handler, StateObject state) { handler?.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReadCallback, state); }

        private void CloseHandler(Socket handler)
        {
            if (handler == null) { return; }

            _clients.Remove(handler);
            try
            {
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception) { LogUtility.LogWarning(_backgroundWorker, "Failed to close the clients handler!"); }
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            _allDone.Set();

            Socket      listener = (Socket)asyncResult.AsyncState;
            Socket      handler  = listener?.EndAccept(asyncResult);
            StateObject state    = new StateObject { WorkSocket = handler };

            _clients.Add(handler);

            Send(handler, _chatLog);
            BeginReceive(handler, state);
        }

        private void ReadCallback(IAsyncResult asyncResult)
        {
            StateObject state   = (StateObject)asyncResult.AsyncState;
            Socket      handler = state?.WorkSocket;

            try
            {
                if (handler != null)
                {
                    int bytesRead = handler.EndReceive(asyncResult);

                    if (bytesRead <= 0) { return; }

                    state.Sb.Append(Encoding.Default.GetString(state.Buffer, 0, bytesRead));
                }

                string content = state?.Sb.ToString();
                _chatLog.Add(content);

                LogUtility.Log(_backgroundWorker, $"Read {content?.Length} bytes from socket. \n Data : {content}");
                foreach (Socket client in _clients) { Send(client, content); }
            }
            catch (Exception)
            {
                LogUtility.LogWarning(_backgroundWorker, "Client forcefully disconnected");
                CloseHandler(handler);
            }
        }

        private void Send(Socket handler, List<string> data)
        {
            foreach (string s in data) { Send(handler, s); }
        }

        private void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.Default.GetBytes(data);

            handler.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, handler);
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                Socket      handler = (Socket)asyncResult.AsyncState;
                StateObject state   = new StateObject { WorkSocket = handler };

                if (handler != null)
                {
                    int bytesSent = handler.EndSend(asyncResult);
                    LogUtility.Log(_backgroundWorker, $"Sent {bytesSent} bytes to client.");
                }

                BeginReceive(handler, state);
            }
            catch (Exception e) { LogUtility.Log(_backgroundWorker, "SendCallbackException: " + e); }
        }
    }
}