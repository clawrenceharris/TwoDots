public class ExplosionRule : IHitRule
{
    public bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string hittableId)
    {
       
        return true;
    }
}