using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionModel : IConnectionModel
{
    private readonly List<IDotPresenter> _path = new();

    /// <summary> Set of unique dot IDs in the path. </summary>
    private readonly HashSet<string> _dotIdsInPath = new();

    /// <summary>The dot IDs to hit from the resulting square connection.</summary>
    private HashSet<string> _dotsToHitFromSquare = new();
    /// <summary> The dot IDs to hit in by the connection from the square.</summary>
    public IReadOnlyList<string> DotsToHitFromSquare => _dotsToHitFromSquare.ToList();

    /// <summary> True if the connection is closed by revisiting an earlier dot.</summary>
    private bool _isSquare;

    private bool _isSessionActive;
    public bool IsSquare => _isSquare;

    public bool IsSessionActive => _isSessionActive;
    private readonly IBoardPresenter _board;
    private readonly IDotConnectionRule _rule;

    public event Action<DotColor> OnColorChanged;
    public event Action<string> OnDotRemovedFromPath;
    public event Action<string> OnDotAddedToPath;
    public event Action OnPathChanged;
    public event Action<IReadOnlyList<string>> OnSquareActivated;
    public event Action<IReadOnlyList<string>> OnSquareDeactivated;
    public event Action<ConnectionResult> OnConnectionCompleted;
    public IReadOnlyList<IDotPresenter> Path => _path;
    public IReadOnlyList<string> DotIdsInPath => _dotIdsInPath.ToList();

    private DotColor _currentColor;
    public DotColor CurrentColor => _currentColor;


   

    public ConnectionModel(IBoardPresenter board, IDotConnectionRule rule)
    {
        _path = new List<IDotPresenter>();
        _dotIdsInPath = new HashSet<string>();
        _dotsToHitFromSquare = new HashSet<string>();
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
        _dotIdsInPath.Add(dot.Dot.ID);
        _isSquare = false;
        _isSessionActive = true;

        OnPathChanged?.Invoke();
        OnDotAddedToPath?.Invoke(dot.Dot.ID);
    }

    public bool TryBacktrack(IDotPresenter dot)
    {
        if (!_isSessionActive || dot == null || _path.Count == 0) return false;

        IDotPresenter head = _path[^1]; // last dot in the path
        if (head.Dot.ID == dot.Dot.ID) return false; // same dot, no-op
        if (_path.Count >= 2 && _path[^2].Dot.ID == dot.Dot.ID)
        {
            if (_isSquare)
            {
                // if the connection is a square, backtrack needs to deactivate the it
                HandleSquareDeactivated();
                return true;
            }
            _dotIdsInPath.Remove(head.Dot.ID);
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
        if (_dotIdsInPath.Contains(dot.Dot.ID))
        {
            if (!_rule.CanConnect(head, dot, new ConnectionResult(this), _board)) return false;
            HandleSquareActivated(dot);


            return true;
        }
        
        // New dot - append if rule allows
        if (!_rule.CanConnect(head, dot, new ConnectionResult(this), _board)) return false;
        _path.Add(dot);
        _dotIdsInPath.Add(dot.Dot.ID);

        OnDotAddedToPath?.Invoke(dot.Dot.ID);
        OnPathChanged?.Invoke();
        return true;
    }


    private void HandleSquareActivated(IDotPresenter dot)
    {
        _path.Add(dot);
        _isSquare = true;
        var dotsToActivate = new List<string>(_dotIdsInPath);

        var dots = _board.GetDotsOnBoard();
        foreach (var d in dots)
        {
            if (dotsToActivate.Contains(d.Dot.ID)) continue;
            if (!d.Dot.DotType.IsColorable()) continue;

            // if the connection color is blank, we can clear any dot
            if (_currentColor.IsBlank())
            {
                dotsToActivate.Add(d.Dot.ID);
                continue;
            }

            if (!d.Dot.TryGetModel<ColorableModel>(out var colorableDot)) continue;
            // We know the connection color is not blank at this point so we can exclude blank dots
            if (colorableDot.Color.IsBlank()) continue;

            if (colorableDot.GetComparableColor(_currentColor) == _currentColor)
            {

                dotsToActivate.Add(d.Dot.ID);
            }


        }
        _dotsToHitFromSquare = new HashSet<string>(dotsToActivate);
        OnDotAddedToPath?.Invoke(dot.Dot.ID);
        OnPathChanged?.Invoke();
        OnSquareActivated?.Invoke(dotsToActivate);
    }
    private void HandleSquareDeactivated()
    {
        _isSquare = false;
        _path.RemoveAt(_path.Count - 1);


        // All the dots that would have been hit from the square excluding the dots that are still in the path
        List<string> dotsToDeactivate = new(_dotsToHitFromSquare.Where(id => !_dotIdsInPath.Contains(id)));
        _dotsToHitFromSquare.Clear();

        OnPathChanged?.Invoke();
        OnSquareDeactivated?.Invoke(dotsToDeactivate);
    }

    private DotColor GetConnectionColor()
    {


        // find the color of the connection
        foreach (var d in _path)
        {
            // skip if not a color dot
            if (!d.Dot.DotType.IsColorable()) continue;
            if (d.Dot.TryGetModel(out ColorableModel colorable))
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

        var payload = new ConnectionResult(this);
        OnConnectionCompleted?.Invoke(payload);
        Cancel();
    }

    public void Cancel()
    {
        _path.Clear();
        _dotIdsInPath.Clear();
        _dotsToHitFromSquare.Clear();
        _isSquare = false;
        _isSessionActive = false;
        _currentColor = DotColor.Blank;
        OnColorChanged?.Invoke(_currentColor);
        OnPathChanged?.Invoke();
    }

}