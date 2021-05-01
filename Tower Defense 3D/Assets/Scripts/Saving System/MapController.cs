using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField] MapSaveManager saveManager;

    private void Awake()
    {
        saveManager.LoadMapData(false);
    }
}
