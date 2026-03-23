/// <summary>
/// Used to defines whether a hittable should be hit if a square has been made in the connection
/// </summary>
public class SquareRule : IRule
{
    public bool CanHit(IBoardPresenter board, Connection connectionSession, string hittableId)
    {
        return connectionSession.IsSquare;
    }
}