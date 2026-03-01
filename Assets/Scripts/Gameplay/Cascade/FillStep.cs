using System.Collections.Generic;
using UnityEngine;

public class FillStep
{
    public FillStepType Type { get; }
    public FillStepPriority Priority { get; }
    public FillStepPhase Phase { get; }
    public IReadOnlyList<string> DotIds { get; }
    public IReadOnlyList<Vector2Int> Positions { get; }
    public string Source { get; }
    public int Sequence { get; internal set; }

    public FillStep(
        FillStepType type,
        FillStepPriority priority,
        FillStepPhase phase,
        IEnumerable<string> dotIds = null,
        IEnumerable<Vector2Int> positions = null,
        string source = null)
    {
        Type = type;
        Priority = priority;
        Phase = phase;
        DotIds = dotIds != null ? new List<string>(dotIds) : new List<string>();
        Positions = positions != null ? new List<Vector2Int>(positions) : new List<Vector2Int>();
        Source = source ?? string.Empty;
    }
}
