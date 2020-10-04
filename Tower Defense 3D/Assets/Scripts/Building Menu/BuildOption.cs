using UnityEngine;
using UnityEngine.EventSystems;

public class BuildOption : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject buildPrefab;

    private GameObject _buildPrefab;

    public void Initialize(BuildOptionData buildOptionData)
    {
        GetComponent<SpriteRenderer>().sprite = buildOptionData.BuildIcon;
        _buildPrefab = buildOptionData.BuildPrefab;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buildPrefab == null)
            return;

        EventManager.ExecuteEvent<IBuildOptionClicked>((x, y) => x.OnBuildOptionClicked(buildPrefab));
    }
}
