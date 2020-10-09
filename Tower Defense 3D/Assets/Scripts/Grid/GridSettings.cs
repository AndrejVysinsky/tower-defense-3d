﻿using System;

[Serializable]
public class GridSettings
{
    public int sizeX;
    public int sizeZ;
    public float cellSize;

    public bool continuousBuilding;
    public bool snapToGrid;
    public bool collisionDetection;
    public bool autoHeight;
}