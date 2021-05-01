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

    public void CheckMap()
    {
        string mapPath = FileManager.MapPath;

        if (Directory.Exists(mapPath) == false)
            Directory.CreateDirectory(mapPath);

        if (File.Exists(mapPath + "defaultGameMap"))
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