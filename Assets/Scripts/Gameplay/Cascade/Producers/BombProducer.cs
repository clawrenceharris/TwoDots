using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PostFill;

    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;
        var bombs = context.Board.GetDotsOnBoard().Where(d => d.Dot.DotType.IsBomb());
        var dotsToHit = new HashSet<string>();
        foreach (var bomb in bombs)
        {
            dotsToHit.Add(bomb.Dot.ID);
            var neighbors = context.Board.GetDotNeighbors(bomb.Dot.GridPosition, true);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (neighbor.Dot.DotType.IsBomb()) continue;
                dotsToHit.Add(neighbor.Dot.ID);
            }
        }
        if (dotsToHit.Count == 0) return;
        outSteps.Add(new FillStep(
                FillStepType.BombExplode,
                FillStepPriority.VeryHigh,
                FillStepPhase.PostFill,
                dotsToHit,
                source: "BombSpawn"));
    }
}