using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void ChangeScene(int buildIndex)
    {
        var sceneName = SceneManager.GetSceneByBuildIndex(buildIndex).name;

        if (sceneName == "Menu Scene" || sceneName == "Editor Scene")
        {
            Destroy(FindObjectOfType<MyNetworkManager>());
        }

        SceneManager.LoadScene(buildIndex);
        
        Time.timeScale = 1;
    }
}
