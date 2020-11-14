using UnityEngine;

public abstract class DynamicTooltip : TooltipBase
{
    [SerializeField] GameObject objectData;

    public GameObject ObjectData
    {
        get
        {
            return objectData;
        }
        set
        {
            objectData = value;
        }
    }
}