using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConnectionModel : IConnectionModel
{
    /// <summary>The history of completed connections. </summary>
    private readonly Stack<ConnectionResult> _connectionHistory = new();
    public Stack<ConnectionResult> ConnectionHistory => _connectionHistory;
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

    public bool IsSquare => _connection.IsSquare;

    private IBoardPresenter _board;

    public IReadOnlyList<string> Path => Connection.Path;
    public IReadOnlyList<string> DotIdsInPath => _dotIdsInPath.ToList();

    private DotColor _currentColor;
    public DotColor CurrentColor => _currentColor;




    public ConnectionModel()
    {
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
        _dotIdsInPath.Add(dot.Dot.ID);
        _square = null;
        _connection.BeginSession(dot);



    }

    public bool TryBacktrack(IDotPresenter dot)
    {
        if (!_connection.IsActive || dot == null || Path.Count == 0) return false;

        var head = Path[^1]; // last dot in the path
        if (head == dot.Dot.ID) return false; // same dot, no-op
        if (Path.Count >= 2 && Path[^2] == dot.Dot.ID)
        {
            if (IsSquare)
            {
                // if the connection is a square, backtrack needs to deactivate the it
               
                _connection.DeactivateSquare();
                _connection.Backtrack();
                return true;
            }
            _dotIdsInPath.Remove(head);
            _square = null;
            _connection.Backtrack();
            return true;
        }
        return false;

    }
    public bool TryAppend(IDotPresenter dot)
    {
        if (!_connection.IsActive || dot == null || Path.Count == 0 || _connection.IsSquare) return false;

        var head = Path[^1]; // last dot in the path
        if (head == dot.Dot.ID) return false; // same dot, no-op

        // Backtrack: new dot is the immediate previous
        if (Path.Count >= 2 && Path[^2] == dot.Dot.ID)
        {
            return false;
        }


        if (dot.Dot.TryGetModel(out Connectable connectable) && connectable.CanConnect(head))
        {
            // Cycle-close: revisiting an earlier dot (not the previous)
            if (_dotIdsInPath.Contains(dot.Dot.ID))
            {
                _connection.Append(dot);
                HandleSquareActivated(dot);
                return true;
            }
            // New dot - append if rule allows
            else
            {
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
    }


    public void UpdateColor()
    {
        _connection.UpdateColor();
    }

    public void End()
    {
        if (!_connection.IsActive) return;
        _connection.EndSession();
        Cancel();
    }

    public void Cancel()
    {
        _dotIdsInPath.Clear();
        _dotsToHitFromSquare.Clear();
        _square = null;
        _currentColor = DotColor.Blank;
    }

}