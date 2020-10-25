using UnityEngine;

public class RangeRenderer
{
    private readonly LineRenderer _lineRenderer;

    private readonly int _vertexCount = 40; // 4 vertices == square
    private readonly float _lineWidth = 0.05f;

    public RangeRenderer(LineRenderer lineRenderer, float radius)
    {
        _lineRenderer = lineRenderer;
        SetupRange(radius);
        HideRange();
    }

    public void ShowRange()
    {
        _lineRenderer.gameObject.SetActive(true);
    }

    public void HideRange()
    {
        _lineRenderer.gameObject.SetActive(false);
    }

    public void ChangeRadius(float radius)
    {
        SetupRange(radius);
    }

    private void SetupRange(float radius)
    {
        _lineRenderer.widthMultiplier = _lineWidth;

        float deltaTheta = (2f * Mathf.PI) / _vertexCount;
        float theta = 0f;

        _lineRenderer.positionCount = _vertexCount;
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0f);
            _lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }
}