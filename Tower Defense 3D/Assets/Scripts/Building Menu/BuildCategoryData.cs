using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Build Category Data", menuName = "Data/Build Category Data")]
public class BuildCategoryData : ScriptableObject
{
    [SerializeField] string categoryName;
    [SerializeField] Sprite categoryIcon;
    [SerializeField] List<BuildOptionData> buildOptions;

    public string CategoryName => categoryName;
    public Sprite CategoryIcon => categoryIcon;
    public List<BuildOptionData> BuildOptions => buildOptions;
}