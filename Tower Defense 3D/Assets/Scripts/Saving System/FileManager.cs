using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class FileManager
{
    public static string SavePath { get; } = Application.persistentDataPath + "/SavedGames/";
    public static string MapPath { get; } = Application.persistentDataPath + "/Maps/";

    public static string[] GetFiles(string path)
    {
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        var filePaths = Directory.GetFiles(path);

        var fileNames = new string[filePaths.Length];

        for (int i = 0; i < filePaths.Length; i++)
        {
            fileNames[i] = System.IO.Path.GetFileNameWithoutExtension(filePaths[i]);
        }

        return fileNames;
    }

    public static void LoadFile<T>(string path, string fileName, out T fileData)
    {
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        if (File.Exists(path + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path + fileName, FileMode.Open);
            fileData = (T)bf.Deserialize(file);
            file.Close();
        }
        else
        {
            fileData = default;
        }
    }

    public static void SaveFile<T>(string path, string fileName, T fileData)
    {
        if (Directory.Exists(path) == false)
            Directory.CreateDirectory(path);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path + fileName);
        bf.Serialize(file, fileData);
        file.Close();
    }
}