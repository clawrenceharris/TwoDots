using System.Collections.Generic;

/// <summary>
/// Result of a connection session when the pointer is released.
/// </summary>
public class ConnectionCompletedPayload
{
    /// <summary>Ordered dot IDs in the path (including cycle-close back to start if applicable).</summary>
    public IReadOnlyList<string> DotIds { get; }

    /// <summary>True if the path was closed by revisiting an earlier dot.</summary>
    public bool IsCycle { get; }

    /// <summary>Number of segments (edges) in the path.</summary>
    public int SegmentCount { get; }

    public ConnectionCompletedPayload(IReadOnlyList<string> dotIds, bool isCycle, int segmentCount)
    {
        DotIds = dotIds;
        IsCycle = isCycle;
        SegmentCount = segmentCount;
    }
}
