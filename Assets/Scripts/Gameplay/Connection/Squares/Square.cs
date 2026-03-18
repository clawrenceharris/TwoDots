using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents a square connection formed by revisiting an earlier dot in a path.
/// Handles logic for determining which dots are affected by the square, activating/deactivating bombs inside the square,
/// and managing lists of affected dot IDs. Provides methods to activate and deactivate square effects,
/// as well as utilities to query dot IDs involved in the square connection.
/// </summary>

public class Square
{
    public List<string> DotsToHit { get; private set; } = new();
    private readonly IBoardPresenter _board;
    private readonly IConnectionModel _connection;
    public readonly Dictionary<string, BombPoolObject> PreviewBombs = new();
    public List<string> DotIdsInSquare { get; private set; } = new();
    public Square(IBoardPresenter board, IConnectionModel connection)
    {
        _board = board;
        _connection = connection;
        
    }
    /// <summary>
    /// Activates the square by selecting the dots to hit and activating any bombs inside the square
    /// </summary>
    public void Activate()
    {
        SelectDotsToHit();
        ActivateBombsInsideSquare();
    }
    /// <summary>
    /// Deactivates the square by removing the dots to hit and deactivating any the bombs inside the square
    /// </summary>
    public void Deactivate()
    {
        DotsToHit.Clear();
        DeactivateBombsInsideSquare();
    }


    /// <summary>
    /// Builds the list of dots that will be hit as a result of the square connection
    /// </summary>
    public void SelectDotsToHit()
    {
        DotsToHit = new List<string>(_connection.DotIdsInPath);

        var dots = _board.GetDotsOnBoard();
        foreach (var d in dots)
        {
            if (DotsToHit.Contains(d.Dot.ID)) continue;
            if (!d.Dot.DotType.IsColorable()) continue;

            // if the connection color is blank, we can clear any dot
            if (_connection.Connection.Color.IsBlank())
            {
                DotsToHit.Add(d.Dot.ID);
                continue;
            }

            if (d.Dot.TryGetModel<Colorable>(out var colorableDot))
            {
                // We know the connection color is not blank at this point so we can exclude blank dots
                if (colorableDot.Color.IsBlank()) continue;

                if (colorableDot.GetComparableColor(_connection.Connection.Color) == _connection.Connection.Color)
                {

                    DotsToHit.Add(d.Dot.ID);
                }
            }
            // if the dot is not a colorable dot but can still be hit by a square then add it to hit list
            else if (d.Dot.DotType.ShouldBeHitBySquare())
            {
                DotsToHit.Add(d.Dot.ID);
            }



        }
    }

   
    /// <summary>
    /// Activates any bombs inside the square by replacing the dots with bombs
    /// </summary>
    private void ActivateBombsInsideSquare()
    {

        DotIdsInSquare = FindDotIdsInsideSquare();
        foreach (string dotId in DotIdsInSquare)
        {
            var dot = _board.GetDot(dotId);
            if (dot == null) continue;
            // 1. Hide original dot visual only
            dot.DotView.gameObject.SetActive(false);

            // 2. Grab a pooled bomb presenter
            var bombPoolObject = PoolService.Instance.GetFromPool<BombPool, BombPoolObject>();
            if (bombPoolObject == null) continue;
            // 3. Position/use it as a preview
            bombPoolObject.Presenter.DotView.transform.position = dot.DotView.transform.position;
            bombPoolObject.Presenter.Spawn(); // purely visual animation
            // 4. Remember it for deactivation/commit
            PreviewBombs[dotId] = bombPoolObject;
        }
    }

    public void DeactivateBombsInsideSquare()
    {

        foreach (string dotId in DotIdsInSquare)
        {
            var dot = _board.GetDot(dotId);
            if (dot == null) continue;

            // 1. Restore original dot visual
            dot.DotView.gameObject.SetActive(true);

            // 2. Hide & return preview bomb to the pool
            if (PreviewBombs.TryGetValue(dotId, out var bombPoolObject))
            {
                bombPoolObject.Presenter.DotView.gameObject.SetActive(false);
                PoolService.Instance.ReturnToPool<BombPool>(bombPoolObject);
            }
        }

        // 3. Clear tracking
        DotIdsInSquare.Clear();
        PreviewBombs.Clear();
    }

    /// <summary>
    /// Builds the list of border dots that form the closed loop of the connection. 
    /// It walks backward through the connection path from the last dot until it reaches the 
    /// previous occurrence of that same dot, returning the dots along that segment 
    /// as the big square border.
    /// </summary>
    /// <returns>List of dot ids that make up the big square border surrounding the inner dots</returns>
    // 
    private List<string> GetSquareBorderDots()
    {
        List<string> square = new();

        for (int i = _connection.Path.Count - 2; i >= 0; i--)
        {
            square.Add(_connection.Path[i].Dot.ID);
            if (_connection.Path[i].Dot.ID == _connection.Path[^1].Dot.ID)
            {
                return square;
            }
        }

        return new List<string>();

    }

    /// <summary>
    /// Scans the entire board and, for each non‑border, non‑edge dot, 
    /// runs BoundaryFill to find its connected region. 
    /// Any region that can reach an un‑bordered board edge is discarded; the remaining 
    /// dots are considered fully enclosed by the square. 
    /// Dots that are part of the connection path itself are excluded.
    /// </summary>
    /// <returns>All dots inside the square that are not in the connection</returns>
    private List<string> FindDotIdsInsideSquare()
    {
        // If the connection is less than 8 dots then it is not big enough, so exit early
        if (_connection.DotIdsInPath.Count < 8)
        {
            return new List<string>();
        }
        var dotsInSquare = new HashSet<string>();
        List<string> squareBorder = GetSquareBorderDots();
        var borderSet = new HashSet<string>(squareBorder);
        var pathSet = new HashSet<string>(_connection.DotIdsInPath);
        var dotsOnBoard = _board.GetDotsOnBoard();
        // Flood-fill from board-edge dots to identify "outside" region, then classify remaining
        // non-border dots as "inside". This avoids classifying isolated outside pockets as inside.
        var outside = new HashSet<string>();
        var queue = new Queue<IDotPresenter>();
        foreach (var dot in dotsOnBoard)
        {
            if (dot == null) continue;
            if (borderSet.Contains(dot.Dot.ID)) continue;
            if (!_board.IsOnEdgeOfBoard(dot.Dot.GridPosition)) continue;
            if (outside.Add(dot.Dot.ID))
            {
                queue.Enqueue(dot);
            }
        }

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            var neighbors = _board.GetDotNeighbors(current.Dot.GridPosition, includesDiagonals: false);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (borderSet.Contains(neighbor.Dot.ID)) continue;
                if (!outside.Add(neighbor.Dot.ID)) continue;
                queue.Enqueue(neighbor);
            }
        }

        foreach (var dot in dotsOnBoard)
        {
            if (dot == null) continue;
            if (borderSet.Contains(dot.Dot.ID)) continue;
            if (outside.Contains(dot.Dot.ID)) continue;
            if (pathSet.Contains(dot.Dot.ID)) continue;
            dotsInSquare.Add(dot.Dot.ID);
        }

        return dotsInSquare.ToList();

    }


    /// <summary>
    /// Commits and completes the square logic by permanently replacing any dots found inside the square with bombs
    /// </summary>
    public void Commit()
    {
        foreach (var dotId in DotIdsInSquare)
        {
            var dot = _board.GetDot(dotId);
            if (dot == null) continue;
            var bomb = PreviewBombs[dotId].Presenter;
            _board.ReplaceDot(dot, bomb);
        }
    }
}
