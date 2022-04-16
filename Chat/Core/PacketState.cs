namespace Chat.Core
{
    public enum PacketState : byte
    {
        ClientConnected,
        SendMessage,
        MessageReceived,
    }
}