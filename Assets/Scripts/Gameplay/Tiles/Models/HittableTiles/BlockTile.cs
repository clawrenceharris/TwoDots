using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BlockTile : Hittable
{
    public BlockTile(BoardEntity entity, int hitMax, List<HitConditionType> conditions, int hitCount = 0) : base(entity, hitMax, conditions, hitCount)
    {
    }

    public override bool ShouldHit()
    {
        if (!ServiceProvider.Instance.TryGetService<ConnectionService>(out var connectionService)) return false;
        var connectionPath = connectionService.ActiveConnection.Path;
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return false;
        var neighbors = boardService.BoardPresenter.GetDotNeighbors(_entity.GridPosition, includesDiagonals: false);
        foreach (var neighbor in neighbors)
        {
            if (connectionPath.Contains(neighbor.Dot.ID))
            {
                return true;
            }
        }
        return false;
    }
}
