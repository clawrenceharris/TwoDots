using System.Linq;
/// <summary>
/// A rule that determines if a hittable entity is adjacent to a connection.
/// It considers all dots selected by a square as also part of the connection.
/// </summary>
public class AdjacentToConnectionRule : IHitRule
{
    public bool CanHit(IBoardPresenter board, Connection connection, string hittableId)
    {
        var hittable = board.GetEntity(hittableId);
        var neighbors = board.GetDotNeighbors(hittable.Entity.GridPosition, includesDiagonals: false);
        var dotsInConnection = connection.Path;
        if (connection.IsSquare)
        {
            dotsInConnection = dotsInConnection.Concat(connection.Square.DotsToHit).Distinct().ToList();
        }
        foreach (var dot in neighbors)
        {
            if (!dotsInConnection.Contains(dot.Dot.ID))
            {
                continue;
            }
            if (!dot.Dot.TryGetModel(out Hittable hittableDot))
            {
                continue;
            }
            if (hittableDot.ShouldClearAfterHit()) return true;
        }
        return false;
    }
}