using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayCheck : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI errorText;
    [SerializeField] SceneLoader sceneLoader;

    private string _mapPath;

    public void Awake()
    {
        _mapPath = Application.persistentDataPath + "/Maps/";
    }

    public void CheckMap()
    {
        if (Directory.Exists(_mapPath) == false)
            Directory.CreateDirectory(_mapPath);

        if (File.Exists(_mapPath + "gamesave.save"))
        {
            errorText.gameObject.SetActive(false);
            sceneLoader.ChangeScene(1);
        }
        else
        {
            errorText.gameObject.SetActive(true);
        }
    }
}