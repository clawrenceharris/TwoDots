public class ExplosionRule : IRule
{
    public bool CanHit(IBoardPresenter board, Connection connectionSession, string hittableId)
    {
       
        return true;
    }
}