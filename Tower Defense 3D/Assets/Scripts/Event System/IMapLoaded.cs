using UnityEngine.EventSystems;

public interface IMapLoaded : IEventSystemHandler
{
    void OnMapBeingLoaded(MapSaveData mapSaveData, bool isLoadingInEditor);
}