namespace ChatNetworking.Core
{
    public interface IChat
    {
        void AddMessage(string userName, string message);
        void AddServerMessage(string message);
    }
}