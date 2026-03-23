using System.Collections.Generic;
using UnityEngine;

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

        var toHit = new List<string>();
        
        for (int x = 0; x < context.Board.Width; x++)
        {
            int y = context.Board.GetBottomMostRow(x);
            var dot = context.Board.GetDotAt(x, y);
            if (dot == null) continue;
            if (dot.Dot.DotType != DotType.Anchor) continue;

            toHit.Add(dot.Dot.ID);
            
        }
        if (toHit.Count == 0) {
            return;
        }

        outSteps.Add(new FillStep(
            FillStepType.AnchorSink,
            FillStepPriority.High,
            FillStepPhase.PostFill,
            toHit: toHit,
            source: "AnchorSink"));
    }
}
