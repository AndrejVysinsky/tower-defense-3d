using UnityEngine;

public interface IEntity
{
    string Name { get; }
    Sprite Sprite { get; }
    int CurrentHitPoints { get; }
    int TotalHitPoints { get; }
}