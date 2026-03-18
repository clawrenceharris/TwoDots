public abstract class DotHitRule : IHitRule
{
    public abstract bool CanHit(IBoardPresenter board, Connection connectionSession, string dotId);
}