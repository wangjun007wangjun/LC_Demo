using BaseFramework;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    public LineRenderer lineRenderer
    {
        get
        {
            _lineRenderer = _lineRenderer ?? GetComponent<LineRenderer>();
            return _lineRenderer;
        }
    }
    
    public void SetRendererColor(Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void DrawSmooth(Vector3[] linePositions, int smooth = 10)
    {
        Vector3[] smoothPoints = LineUtil.GetSmoothPoints(linePositions, smooth);

        lineRenderer.positionCount = smoothPoints.Length;
        lineRenderer.SetPositions(smoothPoints);
    }
    
    public void Clear()
    {
        lineRenderer.positionCount = 0;
    }
}
