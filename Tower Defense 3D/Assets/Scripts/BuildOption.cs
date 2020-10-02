using UnityEngine;
using UnityEngine.EventSystems;

public class BuildOption : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject buildPrefab;

    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager.ExecuteEvent<IBuildOptionClicked>((x, y) => x.OnBuildOptionClicked(buildPrefab));
    }
}
