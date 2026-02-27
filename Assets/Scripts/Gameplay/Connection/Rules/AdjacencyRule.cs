using System.Collections.Generic;

/// <summary>
/// Checks if the connection is valid based on the adjacency of the dots.
/// </summary>
public class AdjacencyRule : IDotConnectionRule
{
    public bool CanConnect(IDotPresenter fromDot, IDotPresenter toDot, ConnectionModel connection, IBoardPresenter board)
    {
        var delta = toDot.Dot.GridPosition - fromDot.Dot.GridPosition;
        if (delta.x == 0 && delta.y == 0) return false;
        if (delta.x == 0) return delta.y == 1 || delta.y == -1;
        if (delta.y == 0) return delta.x == 1 || delta.x == -1;
        return false;
    }
}