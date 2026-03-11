using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Owns connection session state: path building, backtracking, and cycle-close.
/// Uses IDotConnectionRule for connectability; emits events for visuals and completion.
/// </summary>
public class ConnectionPresenter : IConnectionPresenter
{
    private ConnectorLineView _activeDragLine;
    private IConnectionModel _model;
    public static event Action<DotColor> OnColorChanged;
    private readonly Stack<ConnectorLineView> _activeConnectionSegments = new();
   
    private IBoardPresenter _board;
    public Stack<ConnectionResult> ConnectionHistory => _model.ConnectionHistory;
    public ConnectionSession Session => _model.Session;
   
      public void Initialize(IBoardPresenter board)
    {
        _model = new ConnectionModel(board, new BaseConnectionRule());
        _board = board;

        InputRouter.OnPointerDragged += OnPointerDragged;
        InputRouter.OnDotConnected += OnInputDotConnected;
        InputRouter.OnDotSelectionEnded += OnInputDotSelectionEnded;
        InputRouter.OnDotSelected += OnInputDotSelected;
        _model.Session.OnColorChanged += OnConnectionColorChanged;
        _model.Session.OnPathChanged += HandlePathChanged;
        _model.Session.OnSquareActivated += HandleSquareActivated;
        _model.Session.OnSquareDeactivated += HandleSquareDeactivated;
        _model.Session.OnDotRemovedFromPath += HandleDotRemovedFromPath;
    }
    private void HandlePathChanged()
    {
        _model.UpdateColor();
    }

   
    private void HandleDotRemovedFromPath(string dotId)
    {
        var dot = _board.GetDot(dotId);
        if (dot.TryGetPresenter(out IConnectableDotPresenter presenter))
        {
            presenter.Disconnect();
        }
    }
    private void HandleSquareDeactivated(IReadOnlyList<string> dotsToDeactivate)
    {
        foreach (var dotId in dotsToDeactivate)
        {
            var dot = _board.GetDot(dotId);
            if (dot.TryGetPresenter(out IConnectableDotPresenter presenter)) {
                presenter.Deselect();
            }
        }
    }
    private void HandleSquareActivated(IReadOnlyList<string> dotsToActivate)
    {
        foreach (var dotId in dotsToActivate)
        {
            if(_board.GetDot(dotId).TryGetPresenter(out IConnectableDotPresenter presenter)){
                presenter.Select(_model.Session);
            }
        }
    }

    private void OnConnectionColorChanged(DotColor color)
    {
        foreach (var dotId in _model.DotIdsInPath)
        {
            var dot = _board.GetDot(dotId);
            if (dot.TryGetPresenter(out IConnectableDotPresenter presenter))
            {
                presenter.ChangeColor(color);
            }
        }
        OnColorChanged?.Invoke(color);
    }
    private void OnInputDotSelected(DotPresenter dot)
    {
        if ( _model.Session.IsActive) return;
        
        _model.Begin(dot);
        if (dot.TryGetPresenter(out IConnectableDotPresenter presenter))
        {
            presenter.Connect(_model.Session.Color);
        }
        
        
    }

    
    private void OnInputDotConnected(DotPresenter dot)
    {
        if (!_model.Session.IsActive || _model.Path.Count == 0) return;
        var previousDot = _model.Path[^1];

        if (_model.TryAppend(dot))
        {
            
            AddConnectionSegment(previousDot.DotView.transform.position, dot.DotView.transform.position);
    
            if (dot.TryGetPresenter(out IConnectableDotPresenter presenter))
            {
                presenter.Connect(_model.Session.Color);
            }
        }
        else if (_model.TryBacktrack(dot))
        {
            RemoveLastConnectionSegment();
        }

    }
    private void OnPointerDragged(Vector3 worldPos)
    {
        if (!_model.Session.IsActive || _model.Path.Count == 0 || _model.Session.IsSquare)
        {
            HideDragLine();
            return;
        }

        Vector3 from = _model.Path[^1].DotView.transform.position;
        UpdateDragLine(from, worldPos);


    }

    private void OnInputDotSelectionEnded()
    {

        
        foreach (var dotId in _model.DotIdsInPath)
        {
            var dot = _board.GetDot(dotId);
            if (dot.TryGetPresenter(out IConnectableDotPresenter presenter))
            {
                presenter.Disconnect();
            }
        }
        ClearConnectionSegments();
        HideDragLine();
        _model.End();
    }



    /// <summary>
    /// Add one pooled line segment between two world positions (e.g. dot centers).
    /// Uses LinePool; segment is stored for later RemoveLastConnectionSegment or ClearConnectionSegments.
    /// </summary>
    public void AddConnectionSegment(Vector3 fromWorld, Vector3 toWorld)
    {
        ConnectorLineView line = PoolService.Instance.GetFromPool<LinePool, ConnectorLineView>();
        if (line == null) return;
        line.transform.SetParent(ServiceProvider.Instance.GetService<ConnectionService>().transform, true);
        
        line.SetFinalPositions(fromWorld, toWorld);
        line.SetColor(ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(_model.Session.Color));
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
            _activeDragLine.transform.SetParent(ServiceProvider.Instance.GetService<ConnectionService>().transform, true);
        }

        var line = _activeDragLine;
        line.SetColor(ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(_model.Session.Color));
        if (line != null)
        {
            line.SetInitialPositions(fromWorld, toWorld);
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
    

   
}
