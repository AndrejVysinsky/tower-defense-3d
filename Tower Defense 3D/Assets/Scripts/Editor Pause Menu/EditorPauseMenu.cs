using UnityEngine;

public class EditorPauseMenu : MonoBehaviour
{
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject mapExplorerPanel;
    [SerializeField] MapSaveManager mapManager;

    private bool _isMenuActive;

    private void Start()
    {
        _isMenuActive = menuPanel.activeSelf;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        _isMenuActive = !_isMenuActive;

        menuPanel.SetActive(_isMenuActive);
        mapExplorerPanel.SetActive(false);
    }

    public void OpenSaveMapPanel()
    {
        menuPanel.SetActive(false);
        mapExplorerPanel.SetActive(true);
        mapExplorerPanel.GetComponent<MapExplorer>().ShowSavePanel();
    }

    public void OpenLoadMapPanel()
    {
        menuPanel.SetActive(false);
        mapExplorerPanel.SetActive(true);
        mapExplorerPanel.GetComponent<MapExplorer>().ShowLoadPanel();
    }

    public void CallClearMap()
    {
        mapManager.ClearScene();
        mapManager.InitializePathway();
    }

    public void CloseMapPanel()
    {
        menuPanel.SetActive(true);
        mapExplorerPanel.SetActive(false);
    }
}