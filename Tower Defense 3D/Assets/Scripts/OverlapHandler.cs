﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OverlapHandler : MonoBehaviour
{
    private BoxCollider _boxCollider;
    private GameObject _parentObject;

    private List<GameObject> _objectsInRange;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _objectsInRange = new List<GameObject>();
    }

    public void RegisterParent(GameObject parent)
    {
        _parentObject = parent;
    }

    public void ResizeCollider(Vector3 size)
    {
        _boxCollider.size = size;
    }

    public void SetPosition(Vector3 position)
    {
        if (transform.position == position)
            return;

        transform.position = position;

        CheckOverlap();
    }

    private void CheckOverlap()
    {
        var bounds = _boxCollider.bounds;

        foreach (var overlappedObject in _objectsInRange)
        {
            var meshRenderer = overlappedObject.GetComponent<MeshRenderer>();

            meshRenderer.enabled = true;

            float maxTolerance = 0.05f;

            if (IsHigherOrLowerThan(overlappedObject, maxTolerance))
            {
                continue;
            }

            var point = overlappedObject.GetComponent<Collider>().ClosestPoint(transform.position);

            float minX = transform.position.x - bounds.extents.x + maxTolerance;
            float maxX = transform.position.x + bounds.extents.x - maxTolerance;

            float minZ = transform.position.z - bounds.extents.z + maxTolerance;
            float maxZ = transform.position.z + bounds.extents.z - maxTolerance;

            if (point.x > minX && point.x < maxX && point.z > minZ && point.z < maxZ)
            {
                meshRenderer.enabled = false;
            }
        }
    }

    private bool IsHigherOrLowerThan(GameObject gameObject, float maxTolerance)
    {
        var otherBounds = gameObject.GetComponent<Collider>().bounds;

        var upperYOfStaticGameObject = gameObject.transform.position.y + otherBounds.extents.y - maxTolerance;
        var lowerYOfStaticGameObject = gameObject.transform.position.y - otherBounds.extents.y + maxTolerance;

        var upperYOfMovingGameObject = transform.position.y + _boxCollider.bounds.extents.y - maxTolerance;
        var lowerYOfMovingGameObject = transform.position.y - _boxCollider.bounds.extents.y + maxTolerance;

        return lowerYOfMovingGameObject >= upperYOfStaticGameObject || upperYOfMovingGameObject <= lowerYOfStaticGameObject;
    }

    public void RemoveOverlappedObjects()
    {
        var overlapped = _objectsInRange.Where(x => x.GetComponent<MeshRenderer>().enabled == false).ToList();
        overlapped.ForEach(x => Destroy(x));

        _objectsInRange.RemoveAll(x => overlapped.Contains(x));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grid") || other.gameObject == _parentObject)
        {
            return;
        }

        _objectsInRange.Add(other.gameObject);

        //sometimes as soon as object enters range it collides, but CheckOverlap was called too soon (even though LateUpdate should fire after OnTriggerEnter)
        CheckOverlap();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Grid") || other.gameObject == _parentObject)
        {
            return;
        }

        _objectsInRange.Remove(other.gameObject);
    }

    public void SetCollisionDetection(bool detectCollision)
    {
        _objectsInRange.Clear();
        _boxCollider.enabled = detectCollision;
    }
}
