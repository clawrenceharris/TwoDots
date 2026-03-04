using System.Collections.Generic;

/// <summary>
/// Minimal rule that allows any connection for mechanism testing.
/// Replace with adjacency/color rules later.
/// </summary>
public class BaseConnectionRule : IDotConnectionRule
{
    public bool CanConnect(IDotPresenter fromDot, IDotPresenter toDot, IConnectionModel connection, IBoardPresenter board)
    {
        var defaultRules = new List<IDotConnectionRule>
        {
            new AdjacencyRule(),
            new ColorRule(),
        };
        foreach (var rule in defaultRules)
        {
            if (!rule.CanConnect(fromDot, toDot, connection, board)) return false;
        }
        return true;

    }
}
