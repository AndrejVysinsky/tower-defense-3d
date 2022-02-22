using UnityEngine;

public interface IProjectileMovement
{
    void Initialize(Enemy enemy, float effectStrength);
    void MoveInPositionOfTarget();
}