using UnityEngine;

public interface IProjectileMovement
{
    void Initialize(Vector3 targetPosition, float effectStrength);
    void MoveInPositionOfTarget();
}