using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ITowerType
{
    int Level { get; }
    TowerData TowerData { get; }

    void Shoot(GameObject target);
    void Upgrade();
    void Sell();
}