using UnityEngine;

public interface IProjectileSingleTarget : IProjectileMovement
{
    void ApplyEffectOnImpact(GameObject target);
}