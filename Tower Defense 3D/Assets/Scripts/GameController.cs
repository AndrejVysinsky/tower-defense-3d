using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] MapSaveManager saveManager;

    private void Awake()
    {
        saveManager.LoadMapData();
    }
}
