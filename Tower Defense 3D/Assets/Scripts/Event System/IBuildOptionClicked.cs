using UnityEngine;
using UnityEngine.EventSystems;

public interface IBuildOptionClicked : IEventSystemHandler
{
    void OnBuildOptionClicked(GameObject gameObject);
}