
namespace GalaxyShared
{ 
    public interface IMessageHandler
    {
        void HandleMessage(LoginMessage msg, object extra=null);
        void HandleMessage(NewUserMessage msg, object extra=null);
        void HandleMessage(NewUserResultMessage msg, object extra=null);
        void HandleMessage(LoginResultMessage msg, object extra = null);
        void HandleMessage(PlayerStateMessage msg, object extra = null);
        void HandleMessage(GoToWarpMessage msg, object extra = null);
        void HandleMessage(DropOutOfWarpMessage msg, object extra = null);
        void HandleMessage(MiningMessage msg, object extra = null);
        void HandleMessage(InputMessage msg, object extra = null);
        void HandleMessage(Player msg, object extra = null);
        void HandleMessage(Ship msg, object extra = null);
        void HandleMessage(Asteroid msg, object extra = null);
        void HandleMessage(ConstructionMessage msg, object extra = null);
    }
}
