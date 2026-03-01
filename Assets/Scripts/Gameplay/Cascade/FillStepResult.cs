using System.Collections.Generic;
using UnityEngine;

public class FillStepResult
{
    public IReadOnlyList<string> ClearedDotIds { get; }
    public IReadOnlyList<Vector2Int> ClearedPositions { get; }

    public bool HasClears => ClearedDotIds.Count > 0;

    public FillStepResult(IEnumerable<string> clearedDotIds, IEnumerable<Vector2Int> clearedPositions)
    {
        ClearedDotIds = clearedDotIds != null ? new List<string>(clearedDotIds) : new List<string>();
        ClearedPositions = clearedPositions != null ? new List<Vector2Int>(clearedPositions) : new List<Vector2Int>();
    }

    public static FillStepResult Empty => new FillStepResult(null, null);
}
