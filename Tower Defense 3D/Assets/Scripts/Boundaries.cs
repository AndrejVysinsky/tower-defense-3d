using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Boundaries
{
    public int X1 { get; private set; }
    public int X2 { get; private set; }
    public int Z1 { get; private set; }
    public int Z2 { get; private set; }

    public void SetBoundaries(int x1, int x2, int z1, int z2)
    {
        X1 = x1;
        X2 = x2;
        Z1 = z1;
        Z2 = z2;
    }

    public void ExtendBoundariesTo(Boundaries boundaries)
    {
        ExtendBoundariesTo(boundaries.X1, boundaries.X2, boundaries.Z1, boundaries.Z2);
    }

    public void ExtendBoundariesTo(int x1, int x2, int z1, int z2)
    {
        if (x1 < X1)
            X1 = x1;

        if (x2 > X2)
            X2 = x2;

        if (z1 < Z1)
            Z1 = z1;

        if (z2 > Z2)
            Z2 = z2;
    }

    public Vector3 GetMiddlePoint()
    {
        return new Vector3((X1 + X2) / 2.0f, 0, (Z1 + Z2) / 2.0f);
    }
}