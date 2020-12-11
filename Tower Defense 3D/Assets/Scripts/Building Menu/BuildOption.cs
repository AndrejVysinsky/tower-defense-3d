using UnityEngine;
using UnityEngine.EventSystems;

public class BuildOption : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject buildPrefab;

    public GameObject BuildPrefab
    {
        get
        {
            return buildPrefab;
        }
        private set
        {
            buildPrefab = value;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buildPrefab == null)
            return;

        EventManager.ExecuteEvent<IBuildOptionClicked>((x, y) => x.OnBuildOptionClicked(buildPrefab));
    }
}
