/// <summary>
/// Used to define whether a normal or normal-ish dot should be hit.
/// </summary>
public class NormalHitRule : DotHitRule
{
    public override bool CanHit(IBoardPresenter board, Connection connection, string dotId)
    {
        if (connection.Path.Contains(dotId))
        {
            return true;
        }
        if (connection.Square != null)
        {
            if (connection.Square.DotsToHit.Contains(dotId))
            {
                return true;
            }
        }
        return false;
    }
}