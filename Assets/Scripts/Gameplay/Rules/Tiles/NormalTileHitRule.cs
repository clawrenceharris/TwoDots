public class NormalTileHitRule : Rule
{
    public override bool CanHit(IBoardPresenter board, Connection connection, string tileId)
    {
        Rules.Add(new AdjacentToConnectionRule());
        return base.CanHit(board, connection, tileId);
    }
}