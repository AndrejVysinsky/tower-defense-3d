using UnityEngine;

public class EditorPauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject pauseMenuBg;
    [SerializeField] GameObject mapExplorerPanel;
    [SerializeField] MapSaveManager mapManager;

    private bool _isPauseMenuActive;

    private void Start()
    {
        _isPauseMenuActive = pausePanel.activeSelf;
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
        _isPauseMenuActive = !_isPauseMenuActive;

        pausePanel.SetActive(_isPauseMenuActive);
        pauseMenuBg.SetActive(_isPauseMenuActive);
        mapExplorerPanel.SetActive(false);
    }

    public void OpenSaveMapPanel()
    {
        pausePanel.SetActive(false);
        mapExplorerPanel.SetActive(true);
        mapExplorerPanel.GetComponent<MapFileExplorer>().ShowSavePanel();
    }

    public void OpenLoadMapPanel()
    {
        pausePanel.SetActive(false);
        mapExplorerPanel.SetActive(true);
        mapExplorerPanel.GetComponent<MapFileExplorer>().ShowLoadPanel();
    }

    public void CallClearMap()
    {
        mapManager.ClearScene();
    }

    public void CloseMapPanel()
    {
        pausePanel.SetActive(true);
        mapExplorerPanel.SetActive(false);
    }
}