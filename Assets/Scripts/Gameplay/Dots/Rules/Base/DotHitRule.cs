public abstract class DotHitRule : IHitRule
{
    public abstract bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string dotId);
}