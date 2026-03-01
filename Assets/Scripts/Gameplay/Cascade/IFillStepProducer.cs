using System.Collections.Generic;

/// <summary>
/// Implemented by mechanic modules that add fill steps to the cascade. The runner calls
/// CollectSteps at the start of each phase (pre-gravity or post-fill); producers inspect
/// context (e.g. recent clears, connection payload) and append steps to outSteps.
/// </summary>
public interface IFillStepProducer
{
    /// <summary>Phase this producer contributes to (PreGravity or PostFill).</summary>
    FillStepPhase Phase { get; }
    /// <summary>Inspect context and append any applicable fill steps to outSteps.</summary>
    /// <param name="context">Current cascade context (board, recent clears, payload).</param>
    /// <param name="outSteps">List to append new steps to; never null.</param>
    void CollectSteps(CascadeContext context, List<FillStep> outSteps);
}
