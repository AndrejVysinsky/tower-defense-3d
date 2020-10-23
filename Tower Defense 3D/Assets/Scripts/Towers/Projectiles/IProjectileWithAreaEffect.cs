using System.Collections.Generic;
using UnityEngine;

public interface IProjectileWithAreaEffect : IProjectileMovement
{
    void AddTarget(GameObject target);
    void RemoveTarget(GameObject target);
    void ApplyEffectOnImpact(List<GameObject> targets);
}