using UnityEngine.EventSystems;

public interface IMapCleared : IEventSystemHandler
{
    void OnMapBeingCleared();
}