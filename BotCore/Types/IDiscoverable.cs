namespace BotCore.Types
{
    public interface IDiscoverable
    {
        void OnDiscovery(GameClient client);
        void OnRemoved(GameClient client);
        void OnPositionUpdated(GameClient client, Position oldPosition, Position newPosition);
    }
}