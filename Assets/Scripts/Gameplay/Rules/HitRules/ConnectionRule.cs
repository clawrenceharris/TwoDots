public class ConnectionRule : IRule
{
    public bool CanHit(IBoardPresenter board, Connection connection, string hittableId)
    {

       if (connection.Path.Contains(hittableId))
        {
            return true;
        }
        if (connection.IsSquare && connection.Square != null)
        {
            if (connection.Square.AllDotsToHit.Contains(hittableId))
            {
                return true;
            }
        }
        return false;
    }
}