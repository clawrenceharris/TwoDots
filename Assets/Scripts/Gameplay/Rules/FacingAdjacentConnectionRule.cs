using System.Linq;

public class FacingAdjacentConnectionRule : IRule
{
    public bool CanHit(IBoardPresenter board, Connection connection, string hittableId)
    {
        var entity = board.GetEntity(hittableId);
        var neighbors = board.GetDotNeighbors(entity.Entity.GridPosition, includesDiagonals: false);
        var dotsInConnection = connection.Path;
        if (connection.IsSquare)
        {
            dotsInConnection = dotsInConnection.Concat(connection.Square.AllDotsToHit).Distinct().ToList();
        }
        Directional directional = entity.Entity.GetModel<Directional>();
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
            // Check if the dot is directly adjacent to entity's facing direction
            if (directional.FacingDirection + entity.Entity.GridPosition != dot.Dot.GridPosition)
            {
                continue;
            }
            if (hittableDot.ShouldClearAfterHit()) return true;
        }
        return false;
    }
}