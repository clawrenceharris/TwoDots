using System.Collections.Generic;

/// <summary>
/// Post-fill producer that finds gem dots (SquareGem, RectangleGem) adjacent to recently cleared
/// positions and enqueues a step to clear the gem and all of its neighbors (explosion). Runs after
/// gravity/refill; e.g. when a lotus clear exposes a gem, the gem explodes in the same cycle.
/// </summary>
public class GemProducer : IFillStepProducer
{
    /// <inheritdoc />
    public FillStepPhase Phase => FillStepPhase.PostFill;

    /// <inheritdoc />
    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;
        if (context.RecentClearedPositions.Count == 0) return;

        var clearIds = new HashSet<string>();
        foreach (var clearedPos in context.RecentClearedPositions)
        {
            var neighbors = context.Board.GetDotNeighbors<IDotPresenter>(clearedPos, includesDiagonals: true);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (!IsGemType(neighbor.Dot.DotType)) continue;
                clearIds.Add(neighbor.Dot.ID);

                var explosionNeighbors = context.Board.GetDotNeighbors<IDotPresenter>(neighbor.Dot.GridPosition, includesDiagonals: true);
                foreach (var explosionNeighbor in explosionNeighbors)
                {
                    if (explosionNeighbor == null) continue;
                    clearIds.Add(explosionNeighbor.Dot.ID);
                }
            }
        }

        if (clearIds.Count == 0) return;

        outSteps.Add(new FillStep(
            FillStepType.GemExplode,
            FillStepPriority.High,
            FillStepPhase.PostFill,
            toClear: clearIds,
            source: "GemExplosion"));
    }

    private static bool IsGemType(DotType type)
    {
        return type == DotType.SquareGem || type == DotType.RectangleGem;
    }
}
