using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFactory : MonoBehaviour
{
    public ProjectileFactory Instance { get; private set; }

    private Queue<GameObject> _projectiles;

    [SerializeField] GameObject projectilePrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _projectiles = new Queue<GameObject>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //public GameObject GetProjeticle(Vector3 position)
    //{
    //    if (_projectiles.Count > 0 && _projectiles.Peek().activeSelf || _projectiles.Count == 0)
    //    {
    //        return InstantiateProjectile(position);
    //    }
    //}

    //private GameObject InstantiateProjectile(Vector3 position)
    //{
    //    return Instantiate(projectilePrefab, po)
    //}
}
