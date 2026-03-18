using System.Collections.Generic;

/// <summary>
/// Post-fill producer that finds lotus dots with at least one same-color neighbor (cardinal)
/// and enqueues a step to clear the lotus and all matching neighbors. Runs after gravity/refill
/// so new alignments (e.g. after an anchor sink) are detected in the same cycle.
/// </summary>
public class LotusProducer : IFillStepProducer
{
    /// <inheritdoc />
    public FillStepPhase Phase => FillStepPhase.PostFill;

    /// <inheritdoc />
    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;

        foreach (var dot in context.Board.GetDotsOnBoard())
        {
            if (dot == null) continue;
            if (dot.Dot.DotType != DotType.Lotus) continue;
            if (!dot.Dot.TryGetModel(out Colorable lotusColorable)) continue;

            var matchedIds = new HashSet<string> { dot.Dot.ID };
            var neighbors = context.Board.GetDotNeighbors<IDotPresenter>(dot.Dot.GridPosition, includesDiagonals: false);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (!neighbor.Dot.TryGetModel(out Colorable neighborColorable)) continue;
                if (neighborColorable.Color != lotusColorable.Color) continue;
                matchedIds.Add(neighbor.Dot.ID);
            }

            if (matchedIds.Count <= 1) continue;

            outSteps.Add(new FillStep(
                FillStepType.LotusClear,
                FillStepPriority.Normal,
                FillStepPhase.PostFill,
                toClear: matchedIds,
                source: "Lotus"));
        }
    }
}
