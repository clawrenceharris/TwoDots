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
    private readonly Stack<ConnectorLineView> _activeConnectionSegments = new();
   
    private IBoardPresenter _board;
    public Stack<ConnectionResult> ConnectionHistory => _model.ConnectionHistory;
    public Connection Connection => _model.Connection;
    public ConnectionPresenter()
    {
        _model = new ConnectionModel();
    }
    public void Initialize(IBoardPresenter board)
    {
       
        _model.Initialize(board);
        
        _board = board;
       
        InputRouter.OnPointerDragged += OnPointerDragged;
        InputRouter.OnDotConnected += OnInputDotConnected;
        InputRouter.OnDotSelectionEnded += OnInputDotSelectionEnded;
        InputRouter.OnDotSelected += OnInputDotSelected;
        _model.Connection.OnColorChanged += OnConnectionColorChanged;
        _model.Connection.OnPathChanged += HandlePathChanged;
        _model.Connection.OnSquareActivated += HandleSquareActivated;
        _model.Connection.OnSquareDeactivated += HandleSquareDeactivated;
        _model.Connection.OnDotRemovedFromPath += HandleDotRemovedFromPath;
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
                presenter.Select(_model.Connection);
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
    }
    private void OnInputDotSelected(DotPresenter dot)
    {        
        _model.Begin(dot);
        
        if (dot.TryGetPresenter(out IConnectableDotPresenter presenter))
        {
            presenter.Connect(_model.Connection.Color);
        }
        
        
    }

    
    private void OnInputDotConnected(DotPresenter dot)
    {
        Debug.Log("OnInputDotConnected");

        if (_model.Path.Count == 0) return;
        var previousDot = _model.Path[^1];
        Debug.Log("Previous Dot: " + previousDot);
        Debug.Log("Dot: " + dot.Dot.ID);
        if (_model.TryAppend(dot))
        {
            var previousDotView = _board.GetDot(previousDot).DotView;
            AddConnectionSegment(previousDotView.transform.position, dot.DotView.transform.position);
            Debug.Log("Connecting dot: " + dot.Dot.ID);

            if (dot.TryGetPresenter(out IConnectableDotPresenter presenter))
            {
                presenter.Connect(_model.Connection.Color);
            }
        }
        else if (_model.TryBacktrack(dot))
        {
            RemoveLastConnectionSegment();
        }

    }
    private void OnPointerDragged(Vector3 worldPos)
    {
        Debug.Log("OnPointerDragged");
        if ( _model.Path.Count == 0 || _model.Connection.IsSquare)
        {
            HideDragLine();
            return;
        }
        var lastDot = _board.GetDot(_model.Path[^1]);
        Vector3 from = lastDot.DotView.transform.position;
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
        line.SetColor(ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(_model.Connection.Color));
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
        line.SetColor(ServiceProvider.Instance.GetService<ColorSchemeService>().FromDotColor(_model.Connection.Color));
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
