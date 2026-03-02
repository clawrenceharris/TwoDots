using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Result of a connection session
/// </summary>
public class ConnectionContext
{
    /// <summary>Ordered dot IDs in the path.</summary>
    public IReadOnlyList<string> DotIds { get; }

    /// <summary>True if the path was closed by revisiting an earlier dot.</summary>
    public bool IsSquare { get; }

    /// <summary>The ID's of every dot to hit from the resulting connection. (e.g. all dots) </summary>
    public IReadOnlyList<string> AllDotsToHit { get; }

    /// <summary>Number of segments (edges) in the path.</summary>
    public DotColor ConnectionColor { get; }

    /// <summary>The dot IDs to hit from the resulting square connection. (e.g. all dots or all dots of a distinct color)</summary>
    public IReadOnlyList<string> DotsToHitFromSquare { get; }

    public ConnectionContext(IConnectionModel connection)
    {
        DotIds = connection.Path.Select(p => p.Dot.ID).ToList();
        IsSquare = connection.IsSquare;
        ConnectionColor = connection.CurrentColor;
        DotsToHitFromSquare = connection.DotsToHitFromSquare;
        AllDotsToHit = DotsToHitFromSquare.Concat(DotIds).Distinct().ToList();
    }
}
