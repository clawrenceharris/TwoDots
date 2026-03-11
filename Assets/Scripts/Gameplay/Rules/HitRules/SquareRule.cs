public class SquareRule : IHitRule
{
    public bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string hittableId)
    {
        var hittable = board.GetEntity(hittableId);
        if (hittable == null) return false;
        if(connectionSession.Square == null) return false;
        if(connectionSession.Square.DotIdsToHit.Contains(hittableId))
        {
            return true;
        }
        return false;
    }
}