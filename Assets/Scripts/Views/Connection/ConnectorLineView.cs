using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectorLineView : MonoBehaviour
{
    private Color _color;
    public Color Color => _color;
    [SerializeField] private float _width = 0.2f;
    private LineRenderer _lineRenderer;
    public LineRenderer LineRenderer  => _lineRenderer;

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

    }

    private void OnColorChanged(DotColor color)
    {
        SetColor(ColorSchemeManager.FromDotColor(color));
    }

    private void OnDisable()
    {
        ConnectionPresenter.OnColorChanged -= OnColorChanged;
    }

    public void SetColor(Color color)
    {
        _color = color;
        _lineRenderer.material.color = color;
    }
    
    public void SetPositions(Vector3 fromWorld, Vector3 toWorld)
    {
        _lineRenderer.SetPosition(0, fromWorld);
        _lineRenderer.SetPosition(1, toWorld);
    }
}