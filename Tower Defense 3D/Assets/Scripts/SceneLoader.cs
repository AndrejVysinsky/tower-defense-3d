using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
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

    //public void Start()
    //{
    //    var networkManager = NetworkManager.singleton;

    //    if (networkManager == null)
    //        return;

    //    if (SceneManager.GetActiveScene().buildIndex == 0)
    //    {
    //        networkManager.GetComponent<MyLobbyManager>().DisableCallbacks();

    //        Destroy(networkManager.gameObject);
    //        NetworkManager.Shutdown();
    //    }
    //}

    //private void Update()
    //{
    //    var networkManager = GameObject.Find("Network Manager");
    //    if (networkManager != null)
    //    {
    //        Destroy(networkManager);
    //    }
    //}
}
