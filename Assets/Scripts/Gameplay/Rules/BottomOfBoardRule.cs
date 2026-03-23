public class BottomOfBoardRule : IRule
{
    public bool CanHit(IBoardPresenter board, Connection connection, string hittableId)
    {
        var hittable = board.GetEntity(hittableId);
        return board.IsAtBottomOfBoard(hittable.Entity.GridPosition);
    }
}