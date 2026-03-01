using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SeedAdjacencyProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PreGravity;

    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;
        if (context.RecentClearedPositions.Count == 0) return;

        var seedIds = new HashSet<string>();
        foreach (var clearedPos in context.RecentClearedPositions)
        {
            var neighbors = context.Board.GetDotNeighbors<IDotPresenter>(clearedPos, includesDiagonals: false);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (neighbor.Dot.DotType != DotType.Seed) continue;
                seedIds.Add(neighbor.Dot.ID);
            }
        }

        if (seedIds.Count == 0) return;

        outSteps.Add(new FillStep(
            FillStepType.SeedClear,
            FillStepPriority.High,
            FillStepPhase.PreGravity,
            seedIds,
            source: "SeedAdjacency"));
    }
}
