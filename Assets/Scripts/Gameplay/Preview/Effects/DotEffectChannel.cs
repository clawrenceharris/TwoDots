/// <summary>
/// Logical animation channels that can run independently on a dot.
/// This allows combinations such as idle + pending-clear + preview overlays.
/// </summary>
public enum DotEffectChannel
{
    /// <summary>Ambient long-running visuals.</summary>
    Idle,
    /// <summary>Warning visuals (for example, one-hit-left).</summary>
    PendingClear,
    /// <summary>Connection/path-driven temporary overlay visuals.</summary>
    Preview
}
