using System;
using UnityEngine;

[Serializable]
public class GridSettings
{
    [Header("Grid size settings")]
    public int sizeX;
    public int sizeZ;
    public float cellSize;

    [Header("On/off settings")]
    public bool continuousBuilding;
    public bool avoidUnbuildableTerrain;
    public bool buildOnlyOnTerrain; //obsolete, keep for save file integrity
    public bool snapToGrid;
    public bool autoHeight;
    public bool collisionDetection;
    public bool replaceOnCollision;
    public bool editorOnlyDestruction;

    [Header("Brush settings")]
    [Range(1,10)]
    public int brushSize = 1;
    public int brushDensity;
}