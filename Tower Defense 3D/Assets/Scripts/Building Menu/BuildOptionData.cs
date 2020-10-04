using UnityEngine;

[CreateAssetMenu(fileName = "Build Option Data", menuName = "Data/Build Option Data")]
public class BuildOptionData : ScriptableObject
{
    [SerializeField] GameObject buildPrefab;
    [SerializeField] Sprite buildIcon;

    public GameObject BuildPrefab => buildPrefab;
    public Sprite BuildIcon => buildIcon;
}