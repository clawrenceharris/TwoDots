using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Owns connection session state: path building, backtracking, and cycle-close.
/// Uses IDotConnectionRule for connectability; emits events for visuals and completion.
/// </summary>
public class ConnectionPresenter : MonoBehaviour
{
    private ConnectorLineView _activeDragLine;
    private IConnectionModel _model;
    public static event Action<DotColor> OnColorChanged;
    private readonly Stack<ConnectorLineView> _activeConnectionSegments = new();
    public static event Action OnPathChanged;
    public static event Action<IDotPresenter> OnDotSelected;
    public static event Action<IDotPresenter> OnDotDeselected;
    public static event Action<IDotPresenter> OnDotConnected;
    public static event Action<ConnectionCompletedPayload> OnConnectionCompleted;


    private void Start()
    {
        InputRouter.OnPointerDragged += OnPointerDragged;
        InputRouter.OnDotConnected += OnInputDotConnected;
        InputRouter.OnDotSelectionEnded += OnInputDotSelectionEnded;
        InputRouter.OnDotSelected += OnInputDotSelected;
    }
    public void Initialize(IDotConnectionRule rule, IBoardPresenter board)
    {
        _model = new ConnectionModel(board, rule);
        _model.OnConnectionCompleted += HandleConnectionCompleted;
        _model.OnPathChanged += HandlePathChanged;
        _model.OnColorChanged += OnConnectionColorChanged;
    }
    private void HandlePathChanged()
    {
        _model.UpdateColor();
        OnPathChanged?.Invoke();
    }


    private void OnDestroy()
    {
        InputRouter.OnPointerDragged -= OnPointerDragged;
        InputRouter.OnDotConnected -= OnInputDotConnected;
        InputRouter.OnDotSelectionEnded -= OnInputDotSelectionEnded;
        InputRouter.OnDotSelected -= OnInputDotSelected;
        _model.OnColorChanged -= OnConnectionColorChanged;
        _model.OnConnectionCompleted -= HandleConnectionCompleted;
        _model.OnPathChanged -= HandlePathChanged;
    }
    private void OnConnectionColorChanged(DotColor color)
    {
        OnColorChanged?.Invoke(color);
    }
    private void OnInputDotSelected(IDotPresenter dot)
    {
        if (_model.IsSessionActive) return;
        _model.Begin(dot);
        if(dot.TryGetPresenter(out ConnectableDotPresenter presenter))
        {
            presenter.Connect(_model);
        }
        OnDotSelected?.Invoke(dot);
    }

    
    private void OnInputDotConnected(IDotPresenter dot)
    {
        if (!_model.IsSessionActive || _model.Path.Count == 0) return;
        var previousDot = _model.Path[^1];

        if (_model.TryAppend(dot))
        {
            
            AddConnectionSegment(previousDot.View.transform.position, dot.View.transform.position);
    
            if (dot.TryGetPresenter(out ConnectableDotPresenter presenter))
            {
                presenter.Connect(_model);
            }
            OnDotConnected?.Invoke(dot);
        }
        else if (_model.TryBacktrack(dot))
        {
            RemoveLastConnectionSegment();
            OnDotDeselected?.Invoke(previousDot);
        }

    }
    private void OnPointerDragged(Vector3 worldPos)
    {
        if (!_model.IsSessionActive || _model.Path.Count == 0 || _model.IsSquare)
        {
            HideDragLine();
            return;
        }

        Vector3 from = _model.Path[^1].View.transform.position;
        UpdateDragLine(from, worldPos);


    }

    private void OnInputDotSelectionEnded()
    {
        _model.End();
        ClearConnectionSegments();
        HideDragLine();

    }



    /// <summary>
    /// Add one pooled line segment between two world positions (e.g. dot centers).
    /// Uses LinePool; segment is stored for later RemoveLastConnectionSegment or ClearConnectionSegments.
    /// </summary>
    public void AddConnectionSegment(Vector3 fromWorld, Vector3 toWorld)
    {
        ConnectorLineView line = PoolService.Instance.GetFromPool<LinePool, ConnectorLineView>();
        if (line == null) return;
        line.transform.SetParent(transform, true);

        line.SetPositions(fromWorld, toWorld);
        line.SetColor(ColorSchemeService.FromDotColor(_model.CurrentColor));
        _activeConnectionSegments.Push(line);
    }

    /// <summary>
    /// Remove the most recently added connection segment and return it to the pool.
    /// </summary>
    private void RemoveLastConnectionSegment()
    {
        if (_activeConnectionSegments.Count == 0) return;
        ConnectorLineView line = _activeConnectionSegments.Pop();
        PoolService.Instance.ReturnToPool<LinePool>(line);
    }

    /// <summary>
    /// Return all active connection segments to the line pool.
    /// </summary>
    private void ClearConnectionSegments()
    {
        foreach (var line in _activeConnectionSegments)
            PoolService.Instance.ReturnToPool<LinePool>(line);
        _activeConnectionSegments.Clear();
    }

    /// <summary>
    /// Update or create a drag line from a dot to the current pointer position.
    /// </summary>
    private void UpdateDragLine(Vector3 fromWorld, Vector3 toWorld)
    {
        if (_activeDragLine == null)
        {
            _activeDragLine = PoolService.Instance.GetFromPool<LinePool, ConnectorLineView>();
            if (_activeDragLine == null) return;
            _activeDragLine.transform.SetParent(transform, true);
        }

        var line = _activeDragLine;
        line.SetColor(ColorSchemeService.FromDotColor(_model.CurrentColor));
        if (line != null)
        {
            line.SetPositions(fromWorld, toWorld);
        }


    }

    /// <summary>
    /// Hide the current drag line and return it to the pool.
    /// </summary>
    private void HideDragLine()
    {
        if (_activeDragLine == null) return;
        PoolService.Instance.ReturnToPool<LinePool>(_activeDragLine);
        _activeDragLine = null;
    }
    

    private void HandleConnectionCompleted(ConnectionCompletedPayload payload)
    {

        OnConnectionCompleted?.Invoke(payload);
    }
}
