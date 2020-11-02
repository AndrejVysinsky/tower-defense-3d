using UnityEngine;

public class ResourcePath : MonoBehaviour
{
    [Tooltip("Folder path from Resources folder, eg: Terrain/Winter")]
    [SerializeField] string path;

    public string Path => path;
}