public interface IHitRule
{
    bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string hittableId);
}