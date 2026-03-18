using System.Collections.Generic;

/// <summary>
/// Minimal rule that allows any connection for mechanism testing.
/// Replace with adjacency/color rules later.
/// </summary>
public class BaseConnectionRule : IDotConnectionRule
{
    public bool CanConnect(string fromDotId, string toDotId, Connection connectionSession, IBoardPresenter board)
    {
        var fromDot = board.GetDot(fromDotId);
        var toDot = board.GetDot(toDotId);
        var defaultRules = new List<IDotConnectionRule>
        {
            new AdjacencyRule(),
            new ColorRule(),
        };
        foreach (var rule in defaultRules)
        {
            if (!rule.CanConnect(fromDotId, toDotId, connectionSession, board)) return false;
        }
        return true;

    }
}
