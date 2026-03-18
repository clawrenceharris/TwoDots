public class ExplosionRule : IHitRule
{
    public bool CanHit(IBoardPresenter board, Connection connectionSession, string hittableId)
    {
       
        return true;
    }
}