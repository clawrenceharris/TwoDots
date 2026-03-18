using System.Collections.Generic;

/// <summary>
/// Decides whether a dot can be connected from within the current connection.
/// Adjacency, color, and special-dot rules are implemented separately.
/// </summary>
public interface IDotConnectionRule
{
    /// <summary>
    /// Whether the path can be extended (or backtracked/cycle-closed) to include the given dot.
    /// </summary>
    /// <param name="fromDot">Current head of the path (last dot).</param>
    /// <param name="toDot">Candidate dot to connect to.</param>
    /// <param name="currentPath">Ordered path so far (read-only).</param>
    /// <param name="board">Board for spatial/state queries.</param>
    /// <returns>True if the connection is allowed.</returns>
    bool CanConnect(string fromDotId, string toDotId, Connection connectionSession, IBoardPresenter board);
}
