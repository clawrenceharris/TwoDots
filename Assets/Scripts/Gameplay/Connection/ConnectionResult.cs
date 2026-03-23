using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The final result of a connection session
/// </summary>
public readonly struct ConnectionResult
{
    /// <summary>Ordered, unique dot IDs in the path.</summary>
    public IReadOnlyList<string> DotIdsInPath { get; }

    /// <summary>True if the path was closed by revisiting an earlier dot.</summary>
    public bool IsSquare { get; }

    /// <summary>The square that is created when the connection is closed by revisiting an earlier dot.</summary>
    public Square Square { get; }


    /// <summary>Number of segments (edges) in the path.</summary>
    public DotColor ConnectionColor { get; }

    /// <summary>The dot IDs to hit from the resulting square connection. (e.g. all dots or all dots of a distinct color)</summary>
    public IReadOnlyList<string> DotsToHitFromSquare { get; }

    public ConnectionResult(Connection connection)
    {
       
            DotIdsInPath = connection.Path.AsReadOnly();
            IsSquare = connection.IsSquare;
            ConnectionColor = connection.Color;
            Square = connection.Square;
            IsSquare = Square != null;
            DotsToHitFromSquare = Square?.AllDotsToHit.AsReadOnly() ?? new List<string>().AsReadOnly();
       
    }
}
