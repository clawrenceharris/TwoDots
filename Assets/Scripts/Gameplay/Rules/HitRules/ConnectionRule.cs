public class ConnectionRule : IHitRule
{
    public bool CanHit(IBoardPresenter board, Connection connection, string hittableId)
    {
        var entity = board.GetEntity(hittableId);
        var dotsToHit = connection.DotsToHit;
        if (entity == null) return false;
            
           if(dotsToHit.Contains(hittableId))
           {
                return true;
           }
        
        return false;
    }
}