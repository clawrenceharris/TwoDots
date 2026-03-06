using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pre-gravity producer that finds hedgehog dots adjacent (cardinal) to recently cleared positions
/// and enqueues a step to clear (or trigger) them. Hedgehog activation runs before gravity; if
/// multiple hedgehogs are adjacent, they are included in the same step so gravity waits for all.
/// </summary>
public class HedgehogProducer : IFillStepProducer
{
    /// <inheritdoc />
    public FillStepPhase Phase => FillStepPhase.PreGravity;

    /// <inheritdoc />
    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;
        if (context.RecentClearedPositions.Count == 0) return;

        var hedgehogIds = new HashSet<string>();
        foreach (var clearedPos in context.RecentClearedPositions)
        {
            var neighbors = context.Board.GetDotNeighbors<IDotPresenter>(clearedPos, includesDiagonals: false);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (neighbor.Dot.DotType != DotType.Hedgehog) continue;
                hedgehogIds.Add(neighbor.Dot.ID);
            }
        }

        if (hedgehogIds.Count == 0) return;

        outSteps.Add(new FillStep(
            FillStepType.HedgehogCollision,
            FillStepPriority.High,
            FillStepPhase.PreGravity,
            toHit: hedgehogIds,
            toClear: hedgehogIds,
            source: "Hedgehog"));
    }
}
