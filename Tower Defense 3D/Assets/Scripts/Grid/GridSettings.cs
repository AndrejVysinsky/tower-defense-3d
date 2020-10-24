using System;

[Serializable]
public class GridSettings
{
    public int sizeX;
    public int sizeZ;
    public float cellSize;

    public bool continuousBuilding;
    public bool avoidUnbuildableTerrain;
    public bool buildOnlyOnTerrain;
    public bool snapToGrid;
    public bool autoHeight;
    public bool collisionDetection;
    public bool replaceOnCollision;
    public bool editorOnlyDestruction;
}