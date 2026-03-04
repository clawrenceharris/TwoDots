/// <summary>
/// Kind of board action represented by a fill step. Used for tracing and producer logic.
/// </summary>
public enum FillStepType
{
    /// <summary>Dots in the completed connection path are cleared.</summary>
    ConnectionClear,
    /// <summary>Anchor dots at the bottom of the board sink (clear).</summary>
    AnchorSink,
    /// <summary>Lotus dot and same-color neighbors clear together.</summary>
    LotusClear,
    /// <summary>Gem explodes and clears itself and adjacent dots.</summary>
    GemExplode,
    /// <summary>Gravity moves dots down (internal phase, not enqueued by producers).</summary>
    GravityDrop,
    /// <summary>New dots spawn to fill empty cells (internal phase).</summary>
    RefillSpawn,
    /// <summary>Hedgehog(s) activated by adjacency to cleared cells; cleared before gravity.</summary>
    HedgehogCollision,
    /// <summary>Seed dots adjacent to a connection clear alongside the connection.</summary>
    SeedClear,
    /// <summary>Bomb explodes and clears itself and adjacent dots.</summary>
    BombExplode,
}
