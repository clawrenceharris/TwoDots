/// <summary>
/// Phase of the cascade in which a fill step runs. Pre-gravity steps run before gravity/refill;
/// post-fill steps run after gravity and refill within the same cascade cycle.
/// </summary>
public enum FillStepPhase
{
    /// <summary>Runs before gravity and refill (e.g. connection clear, seed clear, hedgehog).</summary>
    PreGravity,

    /// <summary>Runs after gravity and refill (e.g. anchor sink, lotus, gem explosion).</summary>
    PostFill,

}
