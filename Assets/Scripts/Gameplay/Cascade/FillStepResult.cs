using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Result of executing a single fill step: which dot IDs and positions were cleared.
/// Used by the runner to update cascade context (RecentClears) and trigger follow-up producers.
/// </summary>
public class FillStepResult
{
    /// <summary>IDs of dots that were removed.</summary>
    public IReadOnlyList<string> ClearedDotIds { get; }
    /// <summary>Grid positions that were cleared (before removal).</summary>
    public IReadOnlyList<Vector2Int> ClearedPositions { get; }

    /// <summary>True if any dots were cleared.</summary>
    public bool HasClears => ClearedDotIds.Count > 0;

    /// <summary>Builds a result from the cleared IDs and positions.</summary>
    public FillStepResult(IEnumerable<string> clearedDotIds, IEnumerable<Vector2Int> clearedPositions)
    {
        ClearedDotIds = clearedDotIds != null ? new List<string>(clearedDotIds) : new List<string>();
        ClearedPositions = clearedPositions != null ? new List<Vector2Int>(clearedPositions) : new List<Vector2Int>();
    }

    /// <summary>Result with no clears.</summary>
    public static FillStepResult Empty => new FillStepResult(null, null);
}
