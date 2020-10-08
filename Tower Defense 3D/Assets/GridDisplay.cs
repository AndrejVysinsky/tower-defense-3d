using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridDisplay : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject lineContainer;

    private List<LineRenderer> _lines;
    private int _sizeX;
    private int _sizeZ;
    private float _gridCellSize;
    private int _elevation;

    private readonly float _heightOffSet = 0.01f;

    private void Awake()
    {
        _lines = new List<LineRenderer>();
    }

    public void CalculateGrid(int sizeX, int sizeZ, float gridCellSize, int elevation)
    {
        _gridCellSize = gridCellSize;

        _sizeX = (int)(sizeX * (1.0f / _gridCellSize));
        _sizeZ = (int)(sizeZ * (1.0f / _gridCellSize));

        _elevation = elevation;

        int requiredNumberOfLines = _sizeX + _sizeZ + 2;
        if (_lines.Count < requiredNumberOfLines)
        {
            InstantiateRequiredNumberOfLines(requiredNumberOfLines - _lines.Count);
        }

        //hide excess lines
        if (_lines.Count > requiredNumberOfLines)
        {
            for (int i = requiredNumberOfLines; i < _lines.Count; i++)
            {
                _lines[i].gameObject.SetActive(false);
            }
        }

        RenderLines();
    }

    public void ChangeGridElevation(int elevation)
    {
        _elevation = elevation;
        RenderLines();
    }

    private void RenderLines()
    {
        int lineIndex = 0;

        for (int i = 0; i <= _sizeX; i++)
        {
            _lines[lineIndex].SetPosition(0, new Vector3(0, _heightOffSet + _elevation, i * _gridCellSize));
            _lines[lineIndex].SetPosition(1, new Vector3(_sizeX * _gridCellSize, _heightOffSet + _elevation, i * _gridCellSize));
            lineIndex++;
        }

        for (int i = 0; i <= _sizeZ; i++)
        {
            _lines[lineIndex].SetPosition(0, new Vector3(i * _gridCellSize, _heightOffSet + _elevation, 0));
            _lines[lineIndex].SetPosition(1, new Vector3(i * _gridCellSize, _heightOffSet + _elevation, _sizeZ * _gridCellSize));
            lineIndex++;
        }
    }

    private void InstantiateRequiredNumberOfLines(int numberOfLinesToInstantiate)
    {
        for (int i = 0; i < numberOfLinesToInstantiate; i++)
        {
            GameObject line = Instantiate(linePrefab, lineContainer.transform);
            
            _lines.Add(line.GetComponent<LineRenderer>());
        }
    }
}
