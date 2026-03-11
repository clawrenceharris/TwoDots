using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionModel : IConnectionModel
{
    /// <summary>The history of completed connections. </summary>
    private readonly Stack<ConnectionResult> _connectionHistory = new();
    public Stack<ConnectionResult> ConnectionHistory => _connectionHistory;
    private readonly List<IDotPresenter> _path = new();
    private readonly ConnectionSession _session = new();
    public ConnectionSession Session => _session;
    /// <summary> Set of unique dot IDs in the path. </summary>
    private readonly HashSet<string> _dotIdsInPath = new();
    private Square _square;
    public Square Square => _session.Square;
    /// <summary>The dot IDs to hit from the resulting square connection.</summary>
    private HashSet<string> _dotsToHitFromSquare = new();
    /// <summary> The dot IDs to hit in by the connection from the square.</summary>
    public IReadOnlyList<string> DotsToHitFromSquare => _dotsToHitFromSquare.ToList();


    private bool _isSessionActive;


    public bool IsSquare => _session.IsSquare;

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
        _session = new();
        _board = board;
        _rule = rule;
    }



    public void Begin(IDotPresenter dot)
    {
        if (dot == null) return;

        Cancel();
        _path.Add(dot);
        _dotIdsInPath.Add(dot.Dot.ID);
        _square = null;
        _session.BeginSession(dot);
        _isSessionActive = true;


       
    }

    public bool TryBacktrack(IDotPresenter dot)
    {
        if (!_isSessionActive || dot == null || _path.Count == 0) return false;

        IDotPresenter head = _path[^1]; // last dot in the path
        if (head.Dot.ID == dot.Dot.ID) return false; // same dot, no-op
        if (_path.Count >= 2 && _path[^2].Dot.ID == dot.Dot.ID)
        {
            if (IsSquare)
            {
                // if the connection is a square, backtrack needs to deactivate the it
               
                _session.DeactivateSquare();
                _path.RemoveAt(_path.Count - 1);
                _session.Backtrack();
                return true;
            }
            _dotIdsInPath.Remove(head.Dot.ID);
            _path.RemoveAt(_path.Count - 1);
            _square = null;
            _session.Backtrack();
            return true;
        }
        return false;

    }
    public bool TryAppend(IDotPresenter dot)
    {
        if (!_isSessionActive || dot == null || _path.Count == 0 || IsSquare) return false;

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
            if (!_rule.CanConnect(head, dot, this, _board)) return false;
            _path.Add(dot);
            _session.Append(dot);
            HandleSquareActivated(dot);
            

            return true;
        }

        // New dot - append if rule allows
        if (!_rule.CanConnect(head, dot, this, _board)) return false;
        _path.Add(dot);
        _dotIdsInPath.Add(dot.Dot.ID);
        _session.Append(dot);
        return true;
    }


    private void HandleSquareActivated(IDotPresenter dot)
    {
        _square = new Square(_board, this);
        _session.ActivateSquare(_square);
        OnSquareActivated?.Invoke(_square.DotIdsToHit);
    }


    private DotColor GetConnectionColor()
    {


        // find the color of the connection
        foreach (var d in _path)
        {
            // skip if not a color dot
            if (!d.Dot.DotType.IsColorable()) continue;
            if (d.Dot.TryGetModel(out Colorable colorable))
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
        _session.UpdateColor();
    }

    public void End()
    {
        if (!_isSessionActive) return;
        Cancel();
    }

    public void Cancel()
    {
        _path.Clear();
        _dotIdsInPath.Clear();
        _dotsToHitFromSquare.Clear();
        _square = null;
        _isSessionActive = false;
        _currentColor = DotColor.Blank;
        _session.EndSession();
    }

}