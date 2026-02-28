using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionModel : IConnectionModel
{
    private readonly List<IDotPresenter> _path = new();
    private readonly HashSet<string> _pathIds = new();

    /// <summary>
    /// True if the connection is closed by revisiting an earlier dot.
    /// </summary>
    private bool _isSquare;

    private bool _isSessionActive;
    public bool IsSquare => _isSquare;

    public bool IsSessionActive => _isSessionActive;
    public event Action OnPathChanged;
    private readonly IBoardPresenter _board;
    private readonly IDotConnectionRule _rule;
    public IReadOnlyList<IDotPresenter> Path => _path;
    public event Action<DotColor> OnColorChanged;

    public event Action<string> OnDotRemovedFromPath;
    public event Action<string> OnDotAddedToPath;
    public event Action<ConnectionCompletedPayload> OnConnectionCompleted;
    private DotColor _currentColor;
    public DotColor CurrentColor => _currentColor;
    public ConnectionModel(IBoardPresenter board, IDotConnectionRule rule)
    {
        _path = new List<IDotPresenter>();
        _pathIds = new HashSet<string>();
        _isSquare = false;
        _isSessionActive = false;
        _board = board;
        _rule = rule;
    }



    public void Begin(IDotPresenter dot)
    {
        if (dot == null) return;
        Cancel();
        _path.Add(dot);
        _pathIds.Add(dot.Dot.ID);
        _isSquare = false;
        _isSessionActive = true;

        OnPathChanged?.Invoke();
        OnDotAddedToPath?.Invoke(dot.Dot.ID);
    }

    private bool TryBacktrackAfterSquare(IDotPresenter dot)
    {
        if (!_isSessionActive || dot == null || _path.Count == 0) return false;
        IDotPresenter head = _path[^1]; // last dot in the path
        if (head.Dot.ID == dot.Dot.ID) return false; // same dot, no-op
        if (_path.Count >= 2 && _path[^2].Dot.ID == dot.Dot.ID)
        {
            _path.RemoveAt(_path.Count - 1);
            _isSquare = false;
            OnPathChanged?.Invoke();
            return true;
        }
        return false;
    }
    public bool TryBacktrack(IDotPresenter dot)
    {
        if (!_isSessionActive || dot == null || _path.Count == 0) return false;

        IDotPresenter head = _path[^1]; // last dot in the path
        if (head.Dot.ID == dot.Dot.ID) return false; // same dot, no-op
        if (_path.Count >= 2 && _path[^2].Dot.ID == dot.Dot.ID)
        {
            // only remove the head if the cycle is not closed: a closed cycle means the head is still in the connection
            if (_isSquare)
            {
                return TryBacktrackAfterSquare(dot);
            }
            _pathIds.Remove(head.Dot.ID);
            _path.RemoveAt(_path.Count - 1);
            _isSquare = false;
            OnPathChanged?.Invoke();
            OnDotRemovedFromPath?.Invoke(head.Dot.ID);
            return true;
        }
        return false;

    }
    public bool TryAppend(IDotPresenter dot)
    {
        if (!_isSessionActive || dot == null || _path.Count == 0 || _isSquare) return false;

        IDotPresenter head = _path[^1]; // last dot in the path
        if (head.Dot.ID == dot.Dot.ID) return false; // same dot, no-op

        // Backtrack: new dot is the immediate previous
        if (_path.Count >= 2 && _path[^2].Dot.ID == dot.Dot.ID)
        {
            return false;
        }

        // Cycle-close: revisiting an earlier dot (not the previous)
        if (_pathIds.Contains(dot.Dot.ID))
        {
            if (!_rule.CanConnect(head, dot, this, _board)) return false;
            _path.Add(dot);
            _isSquare = true;

            OnDotAddedToPath?.Invoke(dot.Dot.ID);
            OnPathChanged?.Invoke();
            return true;
        }
        
        // New dot - append if rule allows
        if (!_rule.CanConnect(head, dot, this, _board)) return false;
        _path.Add(dot);
        _pathIds.Add(dot.Dot.ID);

        OnDotAddedToPath?.Invoke(dot.Dot.ID);
        OnPathChanged?.Invoke();
        return true;
    }


    private DotColor GetConnectionColor()
    {


        // find the color of the connection
        foreach (var d in _path)
        {
            // skip if not a color dot
            if (!d.Dot.DotType.IsColorable()) continue;
            if (d.Dot.TryGetComponent(out ColorableModel colorable))
            {
                // skip if the dot's color is blank. We only care about dots with a definitive color
                if (colorable.Color.IsBlank()) continue;

                // set color to the first colorable dot found
                return colorable.Color;
            }
        }
        // if no color dot found, return blank color
        return DotColor.Blank;
    }
    public void UpdateColor()
    {

        var newColor = GetConnectionColor();
        if (newColor != _currentColor)
        {
            _currentColor = newColor;
            OnColorChanged?.Invoke(_currentColor);
        }
    }

    public void End()
    {
        if (!_isSessionActive) return;

        var dotIds = new List<string>();
        for (int i = 0; i < _path.Count; i++)
            dotIds.Add(_path[i].Dot.ID);

        int segmentCount = _path.Count == 0 ? 0 : (_isSquare ? _path.Count : _path.Count - 1);
        var payload = new ConnectionCompletedPayload(dotIds, _isSquare, segmentCount);
        OnConnectionCompleted?.Invoke(payload);
        Cancel();
    }

    public void Cancel()
    {
        _path.Clear();
        _pathIds.Clear();
        _isSquare = false;
        _isSessionActive = false;
        _currentColor = DotColor.Blank;
        OnColorChanged?.Invoke(_currentColor);
        OnPathChanged?.Invoke();
    }

}