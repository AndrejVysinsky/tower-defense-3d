using UnityEngine.EventSystems;

public interface IMapSaved : IEventSystemHandler
{
    void OnMapBeingSaved(MapSaveData mapSaveData);
}