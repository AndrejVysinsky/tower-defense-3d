using UnityEngine.EventSystems;

namespace Assets.Scripts.Network
{
    public interface IServerEvents : IEventSystemHandler
    {
        void OnPlayerInitialized(NetworkPlayer networkPlayer);
    }
}
