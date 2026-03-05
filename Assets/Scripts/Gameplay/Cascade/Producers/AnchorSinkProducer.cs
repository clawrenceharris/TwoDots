using System.Collections.Generic;

/// <summary>
/// Post-fill producer that finds anchor dots at the bottom of the board (no dots below) and
/// enqueues a step to clear them. Runs after gravity and refill so anchors that just landed
/// at the bottom sink in the same cycle.
/// </summary>
public class AnchorSinkProducer : IFillStepProducer
{
    /// <inheritdoc />
    public FillStepPhase Phase => FillStepPhase.PostFill;

    /// <inheritdoc />
    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;

        var anchors = new List<string>();
        foreach (var dot in context.Board.GetDotsOnBoard())
        {
            if (dot == null) continue;
            if (dot.Dot.DotType != DotType.Anchor) continue;
            if (!context.Board.IsAtBottomOfBoard(dot.Dot.GridPosition)) continue;
            anchors.Add(dot.Dot.ID);
        }

        if (anchors.Count == 0) return;

        outSteps.Add(new FillStep(
            FillStepType.AnchorSink,
            FillStepPriority.High,
            FillStepPhase.PostFill,
            toClear: anchors,
            source: "AnchorSink"));
    }
}
