using System.Collections.Generic;
using UnityEngine;

public sealed class PreviewContext
{
    public IBoardPresenter Board { get; }
    public Connection Connection { get; }
    public HashSet<string> ActivePathSet { get; }
    public IReadOnlyList<string> ActivePath { get; }
    public bool IsConnectionActive => Connection != null && Connection.IsActive;

    public PreviewContext(
        IBoardPresenter board,
        Connection connection,
        HashSet<string> activePathSet,
        IReadOnlyList<string> activePath)
    {
        Board = board;
        Connection = connection;
        ActivePathSet = activePathSet;
        ActivePath = activePath;
    }

    public bool IsInActivePath(string entityId)
    {
        return !string.IsNullOrEmpty(entityId) && ActivePathSet != null && ActivePathSet.Contains(entityId);
    }

    public bool HasOneHitLeft(BoardEntity entity)
    {
        if (entity == null) return false;
        if (!entity.TryGetModel(out Hittable hittable)) return false;
        return (hittable.HitMax - hittable.HitCount) == 1;
    }

    public bool IsNeighborConnected(BoardEntity entity, bool includeDiagonals = true)
    {
        if (entity == null || Board == null || ActivePathSet == null || ActivePathSet.Count == 0) return false;
        List<IDotPresenter> neighbors = Board.GetDotNeighbors(entity.GridPosition, includeDiagonals);
        foreach (var neighbor in neighbors)
        {
            if (neighbor == null) continue;
            if (ActivePathSet.Contains(neighbor.Dot.ID))
            {
                return true;
            }
        }
        return false;
    }
}
