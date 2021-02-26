using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathway : MonoBehaviour, IMapCleared
{
    [SerializeField] MapSaveManager map;
    [SerializeField] GameObject lineRendererPrefab;
    [SerializeField] GameObject pathwayColliderPrefab;

    [Header("Checkpoint materials")]
    [SerializeField] Material startCheckpointMaterial;
    [SerializeField] Material defaultCheckpointMaterial;
    [SerializeField] Material endCheckpointMaterial;

    private List<GameObject> _checkpoints = new List<GameObject>();
    private List<GameObject> _pathwayColliders = new List<GameObject>();
    private LineRenderer _pathwayLineRenderer;

    private bool _canUpdatePosition;

    public int NumberOfCheckpoints => _checkpoints.Count;

    private void Start()
    {
        if (_pathwayLineRenderer == null)
        {
            var pathwayRendererObject = Instantiate(lineRendererPrefab, map.transform);
            _pathwayLineRenderer = pathwayRendererObject.GetComponent<LineRenderer>();
            _pathwayLineRenderer.positionCount = 0;
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(gameObject);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(gameObject);
    }

    public Vector3 GetPositionOfLastCheckpoint()
    {
        if (_checkpoints.Count == 0)
            return Vector3.zero;

        if (_checkpoints.Count < 2)
            return _checkpoints[0].transform.position;

        if (_canUpdatePosition == false)
            return _checkpoints[_checkpoints.Count - 1].transform.position;

        return _checkpoints[_checkpoints.Count - 2].transform.position;
    }

    public void UpdatePathwayLastPosition(Vector3 newCheckpointPosition)
    {
        if (_checkpoints.Count < 2)
            return;

        if (_canUpdatePosition == false)
            return;

        _pathwayLineRenderer.SetPosition(_pathwayLineRenderer.positionCount - 1, newCheckpointPosition);
    }

    public bool IsCheckpointLast(GameObject checkpoint)
    {
        return _checkpoints[_checkpoints.Count - 1] == checkpoint;
    }

    public void LastCheckpointDestroyed()
    {
        _checkpoints.RemoveAt(_checkpoints.Count - 1);

        if (_pathwayLineRenderer.positionCount > 0)
        {
            _pathwayLineRenderer.positionCount--;
        }

        if (_pathwayColliders.Count > 0 && _pathwayColliders.Count + 1 > _checkpoints.Count)
        {
            //destroy last pathway collider
            Destroy(_pathwayColliders[_pathwayColliders.Count - 1]);
            _pathwayColliders.RemoveAt(_pathwayColliders.Count - 1);
        }

        StartCoroutine(UpdateCheckpointMaterial());

        _canUpdatePosition = false;
    }

    public void AddCheckpoint(GameObject checkpoint)
    {
        _checkpoints.Add(checkpoint);

        var checkpointScript = checkpoint.GetComponent<Checkpoint>();
        if (checkpointScript.CheckpointNumber == 0)
        {
            checkpointScript.CheckpointNumber = _checkpoints.Count;
        }

        _checkpoints = _checkpoints.OrderBy(c => c.GetComponent<Checkpoint>().CheckpointNumber).ToList();

        if (_pathwayLineRenderer == null)
        {
            var pathwayRendererObject = Instantiate(lineRendererPrefab, map.transform);
            _pathwayLineRenderer = pathwayRendererObject.GetComponent<LineRenderer>();
        }

        _pathwayLineRenderer.positionCount = _checkpoints.Count;
        _pathwayLineRenderer.SetPositions(_checkpoints.Select(x => x.transform.position).ToArray());

        _canUpdatePosition = true;
    }

    public int CheckpointPlaced()
    {
        _canUpdatePosition = false;

        StartCoroutine(UpdateCheckpointMaterial());

        if (_checkpoints.Count > 1)
        {
            AddPathwayCollider(_checkpoints[_checkpoints.Count - 2].transform.position,
                                _checkpoints[_checkpoints.Count - 1].transform.position);
        }

        return _checkpoints.Count;
    }

    private IEnumerator UpdateCheckpointMaterial()
    {
        yield return new WaitForEndOfFrame();

        if (_checkpoints.Count > 0)
        {
            _checkpoints[0].GetComponent<Checkpoint>().SetMaterial(startCheckpointMaterial);
        }

        int lastPlacedIndex = 0;
        for (int i = _checkpoints.Count - 1; i > 0; i--)
        {
            if (_checkpoints[i].GetComponent<Checkpoint>().IsPlaced)
            {
                lastPlacedIndex = i;
                _checkpoints[i].GetComponent<Checkpoint>().SetMaterial(endCheckpointMaterial);
                break;
            }
        }

        if (lastPlacedIndex > 1)
        {
            _checkpoints[lastPlacedIndex - 1].GetComponent<Checkpoint>().SetMaterial(defaultCheckpointMaterial);
        }
    }

    private void AddPathwayCollider(Vector3 startPosition, Vector3 endPosition)
    {
        var colliderObject = Instantiate(pathwayColliderPrefab, map.transform);

        colliderObject.transform.position = (startPosition + endPosition) / 2;

        var localScale = colliderObject.transform.localScale;
        localScale.x = Mathf.Abs(startPosition.x - endPosition.x);
        localScale.z = Mathf.Abs(startPosition.z - endPosition.z);

        colliderObject.transform.localScale = localScale;

        _pathwayColliders.Add(colliderObject);
    }

    public void OnMapBeingCleared()
    {
        _checkpoints.Clear();
        _pathwayColliders.Clear();
    }
}

