/// <summary>
/// Used to define whether a normal or normal-ish dot should be hit.
/// </summary>
public class NormalDotHitRule : Rule
{
    public override bool CanHit(IBoardPresenter board, Connection connection, string dotId)
    {
        Rules.Add(new ConnectionRule());
        Rules.Add(new ExplosionRule());
        return base.CanHit(board, connection, dotId);
    }
}