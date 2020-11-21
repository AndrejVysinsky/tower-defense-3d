using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class DirectionRule
{
    [SerializeField] DirectionEnum direction;
    [SerializeField] bool isConnected;

    public DirectionEnum Direction => direction;
    public bool IsConnected => isConnected;
}