public class ConnectionRule : IHitRule
{
    public bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string hittableId)
    {
        var entity = board.GetEntity(hittableId);
        if (entity == null) return false;
            
           if(connectionSession.Path.Contains(hittableId))
           {
                return true;
           }
        
        return false;
    }
}