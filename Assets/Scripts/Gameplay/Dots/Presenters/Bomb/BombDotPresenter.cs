using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BombDotPresenter : BasePresenter, IExplodableDotPresenter
{
    private bool _explosionReady = false;
     
    public static Dictionary<string, List<string>> bombToDotsMap;
    public BombDotPresenter(IDotPresenter dot, IBoardPresenter board) : base(dot, board)
    {
    }

    public void PrepareForExplode(List<string> targetHittables, List<string> bombIds)
    {
        AssignHittablesToBombs(targetHittables, bombIds);
        _explosionReady = true;
    }
    // This version distributes the hittable dots between the bombs as evenly as possible,
    // and ensures each bomb only animates its assigned targets (so no duplicate hits).

    private void AssignHittablesToBombs(List<string> targetHittables, List<string> bombIds)
    {
        if (_explosionReady) return;

        bombToDotsMap = new Dictionary<string, List<string>>();
        foreach (string bombId in bombIds)
        {
            bombToDotsMap[bombId] = new List<string>();
        }

        // Filter valid hittables (not null, not bombs)
        List<string> validHittables = new();
        foreach (string hittableId in targetHittables)
        {
            var hittable = _board.GetDot(hittableId);
            if (hittable == null || hittable.Dot.DotType.IsBomb())
                continue;
            validHittables.Add(hittableId);
        }

        // Build bomb position lookup
        Dictionary<string, Vector3> bombPositions = bombIds
            .ToDictionary(
                id => id,
                id => {
                    var dot = _board.GetDot(id);
                    return dot?.View.transform.position ?? Vector3.zero;
                }
            );
        
        // Assign each hittable exclusively to its closest bomb
        foreach (string hittableId in validHittables)
        {
            var hittable = _board.GetDot(hittableId);
            Vector2 hittablePos = hittable.View.transform.position;

            string nearestBombId = bombIds[0];
            float minDist = Vector2.Distance(hittablePos, bombPositions[nearestBombId]);
            foreach (var bombId in bombIds)
            {
                float dist = Vector2.Distance(hittablePos, bombPositions[bombId]);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearestBombId = bombId;
                }
            }
            bombToDotsMap[nearestBombId].Add(hittableId);
        }

        // Optional: make distribution more even by "round robin", if needed—
        // but for now, strict closest-bomb assignment per requirements.
    }

    public Sequence Explode()
    {
        if (View is not BombView bombView) return null;
        if (bombToDotsMap == null || !bombToDotsMap.ContainsKey(Dot.ID))
            return DOTween.Sequence();

        var sequence = DOTween.Sequence();
        var hittableIds = bombToDotsMap[Dot.ID];
        Debug.Log($"hittableIds for bomb {Dot.ID}: {hittableIds.Count}");

        foreach (var hittableId in hittableIds)
        {
            if (_board.GetDot(hittableId).TryGetPresenter<IHittableDotPresenter>(out var hittable))
            {
                sequence.Join(bombView.DoLineAnimation(hittable));
            }
        }

        return sequence;
    }
}