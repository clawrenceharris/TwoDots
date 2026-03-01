using System.Collections.Generic;
using UnityEngine;

public class HedgehogProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PreGravity;

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
                if (!IsHedgehogType(neighbor.Dot.DotType)) continue;
                hedgehogIds.Add(neighbor.Dot.ID);
            }
        }

        if (hedgehogIds.Count == 0) return;

        outSteps.Add(new FillStep(
            FillStepType.HedgehogCollision,
            FillStepPriority.High,
            FillStepPhase.PreGravity,
            hedgehogIds,
            source: "Hedgehog"));
    }

    private static bool IsHedgehogType(DotType type)
    {
        // Hedgehog placeholder: map to Beetle until a dedicated DotType is introduced.
        return type == DotType.Beetle;
    }
}
