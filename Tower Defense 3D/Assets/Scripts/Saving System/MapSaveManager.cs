using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using static MapSaveData;

public class MapSaveManager : MonoBehaviour
{
    private MapSaveData _mapSaveData;

    private void Awake()
    {
        _mapSaveData = new MapSaveData();
    }

    public void ObjectPlaced(GameObject gameObject, GameObject prefab)
    {
        _mapSaveData.saveableObjects.Add(new SaveableObject()
        {
            id = gameObject.GetInstanceID(),

            positionX = gameObject.transform.position.x,
            positionY = gameObject.transform.position.y,
            positionZ = gameObject.transform.position.z,

            rotationX = gameObject.transform.rotation.eulerAngles.x,
            rotationY = gameObject.transform.rotation.eulerAngles.y,
            rotationZ = gameObject.transform.rotation.eulerAngles.z,

            scaleX = gameObject.transform.localScale.x,
            scaleY = gameObject.transform.localScale.y,
            scaleZ = gameObject.transform.localScale.z,

            resourcePath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab).Replace(".prefab", "").Replace("Assets/Resources/", "")
        });
    }

    public void ObjectRemoved(int gameObjectID)
    {
       _mapSaveData.saveableObjects.RemoveAll(x => x.id == gameObjectID);
    }

    public void LoadData()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            ClearScene();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            _mapSaveData = (MapSaveData)bf.Deserialize(file);
            file.Close();

            CreateObjects();
        }
        else
        {
            Debug.Log("No saved data available!");
        }
    }

    public void SaveData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, _mapSaveData);
        file.Close();
    }

    public void ClearScene()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        _mapSaveData.saveableObjects.Clear();
    }

    private void CreateObjects()
    {
        _mapSaveData.saveableObjects.ForEach(obj =>
        {
            var gameObject = Instantiate(Resources.Load<GameObject>(obj.resourcePath), transform);

            obj.id = gameObject.GetInstanceID();

            var position = gameObject.transform.position;

            position.x = obj.positionX;
            position.y = obj.positionY;
            position.z = obj.positionZ;

            gameObject.transform.position = position;

            gameObject.transform.Rotate(new Vector3(obj.rotationX, obj.rotationY, obj.rotationZ));

            var scale = gameObject.transform.localScale;

            scale.x = obj.scaleX;
            scale.y = obj.scaleY;
            scale.z = obj.scaleZ;

            gameObject.transform.localScale = scale;
        });
    }   
}
