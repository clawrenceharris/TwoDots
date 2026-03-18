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
    private readonly Connection _connection = new();
    public Connection Connection => _connection;
    /// <summary> Set of unique dot IDs in the path. </summary>
    private readonly HashSet<string> _dotIdsInPath = new();
    private Square _square;
    public Square Square => _connection.Square;
    /// <summary>The dot IDs to hit from the resulting square connection.</summary>
    private readonly HashSet<string> _dotsToHitFromSquare = new();
    /// <summary> The dot IDs to hit in by the connection from the square.</summary>
    public IReadOnlyList<string> DotsToHitFromSquare => _dotsToHitFromSquare.ToList();


    private bool _isSessionActive;


    public bool IsSquare => _connection.IsSquare;

    public bool IsSessionActive => _isSessionActive;
    private IBoardPresenter _board;

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




    public ConnectionModel()
    {
        _path = new List<IDotPresenter>();
        _dotIdsInPath = new HashSet<string>();
        _dotsToHitFromSquare = new HashSet<string>();
        _connection = new();
    }
    public void Initialize(IBoardPresenter board)
    {
        _board = board;
    }



    public void Begin(IDotPresenter dot)
    {
        if (dot == null) return;

        Cancel();
        _path.Add(dot);
        _dotIdsInPath.Add(dot.Dot.ID);
        _square = null;
        _connection.BeginSession(dot);
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
               
                _connection.DeactivateSquare();
                _path.RemoveAt(_path.Count - 1);
                _connection.Backtrack();
                return true;
            }
            _dotIdsInPath.Remove(head.Dot.ID);
            _path.RemoveAt(_path.Count - 1);
            _square = null;
            _connection.Backtrack();
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


        if (dot.Dot.TryGetModel(out Connectable connectable) && connectable.CanConnect(head.Dot.ID))
        {
            // Cycle-close: revisiting an earlier dot (not the previous)
            if (_dotIdsInPath.Contains(dot.Dot.ID))
            {
                _path.Add(dot);
                _connection.Append(dot);
                HandleSquareActivated(dot);
                return true;
            }
            // New dot - append if rule allows
            else
            {
                _path.Add(dot);
                _dotIdsInPath.Add(dot.Dot.ID);
                _connection.Append(dot);
                return true;
            }
        }
       
        return false;
    }


    private void HandleSquareActivated(IDotPresenter dot)
    {
        _square = new Square(_board, this);
        _connection.ActivateSquare(_square);
        OnSquareActivated?.Invoke(_square.DotsToHit);
    }


    public void UpdateColor()
    {
        _connection.UpdateColor();
    }

    public void End()
    {
        if (!_isSessionActive) return;
        _connection.EndSession();
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
    }

}