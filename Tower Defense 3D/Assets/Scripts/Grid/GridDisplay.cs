using System.Collections.Generic;
using UnityEngine;

public class GridDisplay : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject lineContainer;

    [SerializeField] GameObject gridBase;

    [SerializeField] bool hideGrid;

    private readonly float _heightOffSet = 0.01f;
    private readonly int _maxGridElevation = 10;

    private List<LineRenderer> _lines;
    private int _sizeX;
    private int _sizeZ;
    private int _gridElevation;
    private float _gridCellSize;

    private void Awake()
    {
        _gridElevation = 0;

        if (hideGrid)
        {
            gridBase.SetActive(false);
        }
    }

    public void CalculateGrid(int sizeX, int sizeZ, float gridCellSize)
    {
        if (hideGrid)
            return;

        CalculateGridBase(sizeX, sizeZ, gridCellSize);
        CalculateGridLines(sizeX, sizeZ, gridCellSize);

        GridUpdated();
    }

    private void CalculateGridBase(int sizeX, int sizeZ, float gridCellSize)
    {
        gridBase.transform.localScale = new Vector3(sizeX, sizeZ, 1);
        gridBase.transform.position = new Vector3(sizeX / 2, 0, sizeZ / 2);
    }

    private void CalculateGridLines(int sizeX, int sizeZ, float gridCellSize)
    {
        if (_lines == null)
            _lines = new List<LineRenderer>();

        _gridCellSize = gridCellSize;

        _sizeX = (int)(sizeX * (1.0f / _gridCellSize));
        _sizeZ = (int)(sizeZ * (1.0f / _gridCellSize));

        int requiredNumberOfLines = _sizeX + _sizeZ + 2;
        if (_lines.Count < requiredNumberOfLines)
        {
            InstantiateRequiredNumberOfLines(requiredNumberOfLines - _lines.Count);
        }

        //hide excess lines
        for (int i = 0; i < _lines.Count; i++)
        {
            if (i < requiredNumberOfLines)
            {
                _lines[i].gameObject.SetActive(true);
            }
            else
            {
                _lines[i].gameObject.SetActive(false);
            }
        }
    }

    private void GridUpdated()
    {
        if (hideGrid)
            return;

        RenderGridBase();
        RenderLines();
    }

    private void RenderGridBase()
    {
        var position = gridBase.transform.position;

        position.y = _gridElevation + _heightOffSet;

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

    public Vector3 GetGridBasePosition()
    {
        return gridBase.transform.position;
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
            GridUpdated();
        }
    }

    public void DecreaseElevation()
    {
        if (_gridElevation > 0)
        {
            _gridElevation--;
            GridUpdated();
        }
    }

    public void ResetElevation()
    {
        _gridElevation = 0;
        GridUpdated();
    }

    public int GetGridElevation()
    {
        return _gridElevation;
    }
}
