using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A single unit of work in the cascade: clear these dots (by ID) in this phase with this priority.
/// Steps are enqueued by producers and dequeued by the runner in priority order; execution clears
/// dots from the board and can trigger further steps from the same or other producers.
/// </summary>
public class FillStep
{
    /// <summary>Kind of action (ConnectionClear, SeedClear, etc.).</summary>
    public FillStepType Type { get; }
    /// <summary>Ordering within phase; higher runs first.</summary>
    public FillStepPriority Priority { get; }
    
    /// <summary>PreGravity (before gravity/refill) or PostFill (after).</summary>
    public FillStepPhase Phase { get; }
    /// <summary>Dot IDs to hit when this step is executed.</summary>
    public IReadOnlyList<string> ToHit { get; }
    /// <summary>Dot IDs to explode when this step is executed.</summary>
    public IReadOnlyList<string> ToExplode { get; }
    /// <summary>Dot IDs to clear when this step is executed.</summary>
    public IReadOnlyList<string> ToClear { get; }
    /// <summary> Tile IDs to hit when this step is executed.</summary>
    public IReadOnlyList<string> TileIds { get; }
    /// <summary>Optional grid positions; may be used by producers for spatial logic.</summary>
    public IReadOnlyList<Vector2Int> Positions { get; }
    /// <summary>Label for debugging (e.g. "Connection", "SeedAdjacency").</summary>
    public string Source { get; }
    /// <summary>Enqueue order; used to break priority ties. Set by the queue.</summary>
    public int Sequence { get; internal set; }

    /// <summary>Creates a fill step for the given type, priority, phase, and optional dot IDs/positions/source.</summary>
    public FillStep(
        FillStepType type,
        FillStepPriority priority,
        FillStepPhase phase,
        IEnumerable<string> toHit = null,
        IEnumerable<string> toClear = null,
        IEnumerable<string> toExplode = null,
        IEnumerable<string> tileIds = null,
        IEnumerable<Vector2Int> positions = null,
        string source = null)
    {
        Type = type;
        Priority = priority;
        Phase = phase;
        TileIds = tileIds != null ? new List<string>(tileIds) : new List<string>();
        ToHit = toHit != null ? new List<string>(toHit) : new List<string>();
        ToClear = toClear != null ? new List<string>(toClear) : new List<string>();
        ToExplode = toExplode != null ? new List<string>(toExplode) : new List<string>();
        Positions = positions != null ? new List<Vector2Int>(positions) : new List<Vector2Int>();
        Source = source ?? string.Empty;
    }
}
