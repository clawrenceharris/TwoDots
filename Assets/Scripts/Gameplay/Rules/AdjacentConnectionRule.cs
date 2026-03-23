using System.Linq;
using UnityEngine;
/// <summary>
/// A rule that determines if a hittable entity is adjacent to a connection.
/// It considers all dots selected by a square as also part of the connection.
/// </summary>
public class AdjacentToConnectionRule : IRule
{
    public bool CanHit(IBoardPresenter board, Connection connection, string hittableId)
    {
        var hittable = board.GetEntity(hittableId);
        var neighbors = board.GetDotNeighbors(hittable.Entity.GridPosition, includesDiagonals: false);
        var dotsInConnection = connection.Path;
        if (connection.IsSquare)
        {
            dotsInConnection = dotsInConnection.Concat(connection.Square.AllDotsToHit).Distinct().ToList();
        }
        foreach (var dot in neighbors)
        {
            if (!dotsInConnection.Contains(dot.Dot.ID))
            {
                Debug.Log($"Hittable {dot.Dot.ID} is not in the connection");
                continue;
            }
            if (!dot.Dot.TryGetModel(out Hittable hittableDot))
            {
                Debug.Log($"Hittable {dot.Dot.ID} does not have a hittable model");
                continue;
            }
            if (hittableDot.ShouldClearAfterHit()) {
                Debug.Log($"Hittable {dot.Dot.ID} should clear after hit");
                return true;
            }
        }
        return false;
    }
}