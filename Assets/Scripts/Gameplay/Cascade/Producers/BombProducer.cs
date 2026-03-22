using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BombProducer : IFillStepProducer
{
    public FillStepPhase Phase => FillStepPhase.PostFill;

    public void CollectSteps(CascadeContext context, List<FillStep> outSteps)
    {
        if (context == null || outSteps == null) return;
        var dots = context.Board.GetDotsOnBoard();

        var bombIds = new HashSet<string>();
        var dotsToHit = new HashSet<string>();
        foreach (var dot in dots)
        {
            if(dot.Dot.DotType.IsBomb())
            {
                if (context.ClearedDotIds.Contains(dot.Entity.ID)) continue;
                bombIds.Add(dot.Entity.ID);
                var neighbors = context.Board.GetNeighbors(dot.Entity.GridPosition, true);
                neighbors.Sort((a, b) => Vector2Int.Distance(a.GridPosition, dot.Entity.GridPosition)
                .CompareTo(Vector2Int.Distance(b.GridPosition, dot.Entity.GridPosition)));
                foreach (var neighbor in neighbors)
                {
                    if (neighbor == null) continue;
                    if (neighbor.GetEntityType<DotType>() == DotType.Bomb) continue;
                    if (context.ClearedDotIds.Contains(neighbor.ID)) continue;
                    dotsToHit.Add(neighbor.ID);
                }
            }
        }
        if (bombIds.Count == 0) return;
        Debug.Log($"BombProducer: bombIds: {bombIds.Count}, dotsToHit: {dotsToHit.Count}");
        outSteps.Add(new FillStep(
                FillStepType.BombExplode,
                FillStepPriority.VeryHigh,
                FillStepPhase.PostFill,
                toHit: dotsToHit,
                toExplode: bombIds,
                source: "BombSpawn"));
    }
}