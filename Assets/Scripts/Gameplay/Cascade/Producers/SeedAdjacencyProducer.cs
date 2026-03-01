using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Pre-gravity producer that finds seed dots adjacent (cardinal) to any recently cleared position
/// and enqueues a single step to clear them. Seeds cannot be connected; they clear as if part of
/// the connection when next to a cleared cell, before gravity runs.
/// </summary>
public class SeedAdjacencyProducer : IFillStepProducer
{
    /// <inheritdoc />
    public FillStepPhase Phase => FillStepPhase.PreGravity;

    /// <inheritdoc />
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
