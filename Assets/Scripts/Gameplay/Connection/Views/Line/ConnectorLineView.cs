using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectorLineView : MonoBehaviour
{
    private Color _color;
    public Color Color => _color;
    [SerializeField] private float _width = 0.2f;
    private LineRenderer _lineRenderer;
    public LineRenderer LineRenderer => _lineRenderer;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    private void Start()
    {
        _lineRenderer.startWidth = _width;
        _lineRenderer.endWidth = _width;

        _lineRenderer.useWorldSpace = true;
        _lineRenderer.positionCount = 2;
    }
    private void OnEnable()
    {
        ConnectionPresenter.OnColorChanged += OnColorChanged;
        _lineRenderer.sortingLayerName = "Lines";
        _lineRenderer.sortingLayerID = SortingLayer.NameToID("Lines");
    }

    private void OnColorChanged(DotColor color)
    {
        SetColor(ColorSchemeService.FromDotColor(color));
    }

    private void OnDisable()
    {
        ConnectionPresenter.OnColorChanged -= OnColorChanged;
        _lineRenderer.sortingLayerName = "Lines";
        _lineRenderer.sortingLayerID = SortingLayer.NameToID("Lines");
    }

    public void SetColor(Color color)
    {
        _color = color;
        _lineRenderer.material.color = color;
    }

    public void SetFinalPositions(Vector3 from, Vector3 to)
    {
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, to);
        _lineRenderer.sortingLayerName = "Connected Lines";


    }
    public void SetInitialPositions(Vector3 from, Vector3 to)
    {
        _lineRenderer.SetPosition(0, from);
        _lineRenderer.SetPosition(1, to);
    }
}