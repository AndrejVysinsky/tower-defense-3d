using UnityEngine;

public interface IProjectileMovement
{
    void Initialize(uint playerId, Enemy enemy, float effectStrength);
    void MoveInPositionOfTarget();
}