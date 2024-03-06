using SignalRChat.Classes;

namespace SignalRChat.Hubs
{
    public interface IChat
    {
        Task GetMessage(Message message);
        Task ReceivePrivateMessage(Message privateMessages);

        Task GetUsers(List<Connection> connections);
        Task SonidoPalabrota();
    }
}
