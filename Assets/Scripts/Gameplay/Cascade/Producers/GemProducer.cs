using System.Collections.Generic;

public class GemProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PostFill;

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
            clearIds,
            source: "GemExplosion"));
    }

    private static bool IsGemType(DotType type)
    {
        return type == DotType.SquareGem || type == DotType.RectangleGem;
    }
}
