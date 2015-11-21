
namespace GalaxyShared
{ 
    public interface IMessageHandler
    {
        void HandleMessage(LoginMessage msg, object Client);
        void HandleMessage(NewUserMessage msg, object Client);
    }
}
