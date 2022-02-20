using UnityEngine.EventSystems;

namespace Assets.Scripts.Network
{
    public interface IPlayerEvents : IEventSystemHandler
    {
        void OnCurrencyUpdated(uint playersNetId, int currentValue);
        void OnLivesUpdated(uint plyersNetId, int currentValue);
        void OnCreepsUpdated(uint plyersNetId, int currentValue);
    }
}
