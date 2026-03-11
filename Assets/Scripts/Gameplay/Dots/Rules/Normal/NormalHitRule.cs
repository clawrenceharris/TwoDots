public class NormalHitRule : DotHitRule
{
    public override bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string dotId)
    {
        if(connectionSession.Path.Contains(dotId))
        {
            return true;
        }
        if(connectionSession.Square != null)
        {
            if(connectionSession.Square.DotIdsToHit.Contains(dotId))
            {
                return true;
            }
        }
        return false;
    }
}