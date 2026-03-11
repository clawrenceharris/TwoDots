public class AdjacentToConnectionRule : IHitRule
{
    public bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string hittableId)
    {
        var hittable = board.GetEntity(hittableId);
        if (hittable == null) return false;
        var neighbors = board.GetNeighbors(hittable.GetEntity().GridPosition, includesDiagonals: false);
        foreach (var neighbor in neighbors)
        {
            if (neighbor == null) continue;
            if (connectionSession.Path.Contains(neighbor.ID))
            {
                return true;
            }
        }
        return false;
    }
}