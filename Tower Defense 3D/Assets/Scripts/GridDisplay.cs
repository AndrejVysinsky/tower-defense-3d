using System.Collections.Generic;
using UnityEngine;

public class GridDisplay : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject lineContainer;

    [SerializeField] GameObject gridBase;

    private readonly float _heightOffSet = 0.01f;
    private readonly int _maxGridElevation = 3;

    private List<LineRenderer> _lines;
    private int _sizeX;
    private int _sizeZ;
    private int _gridElevation;
    private float _gridCellSize;

    private void Awake()
    {
        _lines = new List<LineRenderer>();
    }

    public void CalculateGrid(int sizeX, int sizeZ, float gridCellSize, int elevation)
    {
        _gridCellSize = gridCellSize;

        _sizeX = (int)(sizeX * (1.0f / _gridCellSize));
        _sizeZ = (int)(sizeZ * (1.0f / _gridCellSize));

        _gridElevation = elevation;

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

        var gridBaseScale = gridBase.transform.localScale;
        
        gridBaseScale.x = _sizeX;
        gridBaseScale.y = _sizeZ;

        gridBase.transform.localScale = gridBaseScale;

        RenderGridBase();
        RenderLines();
    }
    
    private void RenderGridBase()
    {
        var position = gridBase.transform.position;

        position.x = gridBase.transform.localScale.x / 2;
        position.z = gridBase.transform.localScale.y / 2;

        position.y = _gridElevation + _heightOffSet;
        Debug.Log(position.y);

        gridBase.transform.position = position;
    }

    private void RenderLines()
    {
        int lineIndex = 0;

        for (int i = 0; i <= _sizeX; i++)
        {
            _lines[lineIndex].SetPosition(0, new Vector3(0, _heightOffSet + _gridElevation, i * _gridCellSize));
            _lines[lineIndex].SetPosition(1, new Vector3(_sizeX * _gridCellSize, _heightOffSet + _gridElevation, i * _gridCellSize));
            lineIndex++;
        }

        for (int i = 0; i <= _sizeZ; i++)
        {
            _lines[lineIndex].SetPosition(0, new Vector3(i * _gridCellSize, _heightOffSet + _gridElevation, 0));
            _lines[lineIndex].SetPosition(1, new Vector3(i * _gridCellSize, _heightOffSet + _gridElevation, _sizeZ * _gridCellSize));
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

    public void IncreaseElevation()
    {
        if (_gridElevation < _maxGridElevation)
        {
            _gridElevation++;
            GridElevationChanged();
        }
    }

    public void DecreaseElevation()
    {
        if (_gridElevation > 0)
        {
            _gridElevation--;
            GridElevationChanged();
        }
    }

    public void ResetElevation()
    {
        _gridElevation = 0;
        GridElevationChanged();
    }

    public int GetGridElevation()
    {
        return _gridElevation;
    }

    public void GridElevationChanged()
    {
        RenderGridBase();
        RenderLines();
    }
}
