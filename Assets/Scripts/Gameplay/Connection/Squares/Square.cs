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

    private readonly HashSet<IDotPresenter> visitedDots = new();
    public List<string> DotIdsToHit { get; private set; } = new();
    private readonly IBoardPresenter _board;
    private readonly IConnectionModel _connection;
    public readonly Dictionary<string, BombPoolObject> PreviewBombs = new();
    public List<string> DotIdsInSquare { get; private set; } = new();
    public Square(IBoardPresenter board, IConnectionModel connection)
    {
        _board = board;
        _connection = connection;
        SelectDotsToHit();
    }
    public void Activate()
    {
        SelectDotsToHit();
        ActivateBombsInsideSquare();
    }
    public void Deactivate()
    {
        RemoveDotIdsFromSquare();
        DeactivateBombsInsideSquare();
    }


    /// <summary>
    /// Builds the list of dots that will be hit as a result of the square connection
    /// </summary>
    public void SelectDotsToHit()
    {
        DotIdsToHit = new List<string>(_connection.DotIdsInPath);

        var dots = _board.GetDotsOnBoard();
        foreach (var d in dots)
        {
            if (DotIdsToHit.Contains(d.Dot.ID)) continue;
            if (!d.Dot.DotType.IsColorable()) continue;

            // if the connection color is blank, we can clear any dot
            if (_connection.CurrentColor.IsBlank())
            {
                DotIdsToHit.Add(d.Dot.ID);
                continue;
            }

            if (d.Dot.TryGetModel<ColorableModel>(out var colorableDot))
            {
                // We know the connection color is not blank at this point so we can exclude blank dots
                if (colorableDot.Color.IsBlank()) continue;

                if (colorableDot.GetComparableColor(_connection.CurrentColor) == _connection.CurrentColor)
                {

                    DotIdsToHit.Add(d.Dot.ID);
                }
            }
            // if the dot is not a colorable dot but can still be hit by a square then add it to hit list
            else if (d.Dot.DotType.ShouldBeHitBySquare())
            {
                DotIdsToHit.Add(d.Dot.ID);
            }



        }
    }

    /// <summary>
    /// Removes dot ids that would of been hit by the square.
    /// </summary>
    public void RemoveDotIdsFromSquare()
    {
        DotIdsToHit.Clear();
    }

    public void ActivateBombsInsideSquare()
    {

        DotIdsInSquare = FindDotIdsInsideSquare();
        foreach (string dotId in DotIdsInSquare)
        {
            var dot = _board.GetDot(dotId);
            if (dot == null) continue;
            // 1. Hide original dot visual only
            dot.View.gameObject.SetActive(false);

            // 2. Grab a pooled bomb presenter
            var bombPoolObject = PoolService.Instance.GetFromPool<BombPool, BombPoolObject>();
            if (bombPoolObject == null) continue;
            // 3. Position/use it as a preview
            bombPoolObject.Presenter.View.transform.position = dot.View.transform.position;
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
            dot.View.gameObject.SetActive(true);

            // 2. Hide & return preview bomb to the pool
            if (PreviewBombs.TryGetValue(dotId, out var bombPoolObject))
            {
                bombPoolObject.Presenter.View.gameObject.SetActive(false);
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
        HashSet<string> dotsInSquare = new();
        List<string> squareBorder = GetSquareBorderDots();
        for (int col = 0; col < _board.Width; col++)
        {
            for (int row = 0; row < _board.Height; row++)
            {
                IDotPresenter dot = _board.GetDotAt(col, row);
                // Make sure dot is not in the square and not on edge of board since there is no need to fill these
                if (dot != null && !squareBorder.Contains(dot.Dot.ID) && !_board.IsOnEdgeOfBoard(dot.Dot.GridPosition))
                {
                    //Add each valid and unique dot found in the flood fill to the dots in square set and disregard dots within the current connection
                    dotsInSquare.UnionWith(BoundaryFill(dot, squareBorder).Where((dotId) => !_connection.DotIdsInPath.Contains(dotId)));
                }
            }
        }

        return dotsInSquare.ToList();

    }



    /// <summary>
    /// Performs a BFS flood‑fill from startDot, stopping if the region ever touches 
    /// a board edge that is not part of the square border. In that case, the entire region is 
    /// considered ‘outside’ the <paramref name="squareBorder"/> and an empty list is returned. 
    /// Otherwise, returns the IDs of all connected dots reachable from <paramref name="startDot"/> without 
    /// crossing the border.
    /// </summary>
    /// <param name="startDot">The starting dot from which the boundary fill algorithm begins.</param>
    /// <param name="squareBorder">A list of dot ids representing the boundaries of the square area to be filled.</param>
    /// <returns>A list of dots that are inside the square area, or possibly an empty list if no dots found.</returns>
    private List<string> BoundaryFill(IDotPresenter startDot, List<string> squareBorder)
    {
        //list of dot ids that are inside the square
        List<string> insideSquare = new();


        // Queue for breadth-first search traversal
        Queue<IDotPresenter> queue = new();

        // Enqueue the starting dot
        queue.Enqueue(startDot);

        // Perform breadth-first search
        while (queue.Count > 0)
        {
            IDotPresenter currentDot = queue.Dequeue();

            insideSquare.Add(currentDot.Dot.ID);

            visitedDots.Add(currentDot);

            // Get neighbors of the current dot
            var neighbors = _board.GetDotNeighbors(currentDot.Dot.GridPosition, true);

            foreach (var neighbor in neighbors)
            {
                bool isOnEdge = _board.IsOnEdgeOfBoard(neighbor.Dot.GridPosition);
                bool isBorder = squareBorder.Contains(neighbor.Dot.ID);

                if (visitedDots.Contains(neighbor))
                {
                    continue;
                }
                // If the neighbor is:
                //  1. on the edge of the board and 
                //  2. not part of the square border
                // then we know that it cant be inside the square
                if (isOnEdge && !isBorder)
                {
                    //then return empty list
                    return new();
                }

                // If the neighbor is not a dot in the square border, then add it to the queue for further exploration
                // otherwise continue checking the other neighbors of the current dot
                if (!isBorder)
                {

                    queue.Enqueue(neighbor);
                }


                visitedDots.Add(neighbor);

            }

        }

        return insideSquare;
    }

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
