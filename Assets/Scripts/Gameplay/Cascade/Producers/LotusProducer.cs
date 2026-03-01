using System.Collections.Generic;

public class LotusProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PostFill;

    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;

        foreach (var dot in context.Board.GetDotsOnBoard())
        {
            if (dot == null) continue;
            if (dot.Dot.DotType != DotType.Lotus) continue;
            if (!dot.Dot.TryGetComponent(out ColorableModel lotusColorable)) continue;

            var matchedIds = new HashSet<string> { dot.Dot.ID };
            var neighbors = context.Board.GetDotNeighbors<IDotPresenter>(dot.Dot.GridPosition, includesDiagonals: false);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (!neighbor.Dot.TryGetComponent(out ColorableModel neighborColorable)) continue;
                if (neighborColorable.Color != lotusColorable.Color) continue;
                matchedIds.Add(neighbor.Dot.ID);
            }

            if (matchedIds.Count <= 1) continue;

            outSteps.Add(new FillStep(
                FillStepType.LotusClear,
                FillStepPriority.Normal,
                FillStepPhase.PostFill,
                matchedIds,
                source: "Lotus"));
        }
    }
}
