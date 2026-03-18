using UnityEngine;

public class HittableNormalDot : Hittable
{
    public HittableNormalDot(BoardEntity entity, int hitMax = 1, int hitCount = 0) : base(entity, hitMax, hitCount)
    {
    }
    public HittableNormalDot(BoardEntity entity, Clearable clearable,int hitMax = 1,  int hitCount = 0) : base(entity, clearable, hitMax, hitCount)
    {
    }

    public override bool ShouldHit()
    {
        var connectionService = ServiceProvider.Instance.GetService<ConnectionService>();
        Connection connection = connectionService.ActiveConnection;
        if (connection.Path.Contains(_entity.ID))
        {
            return true;
        }
        if (connection.IsSquare)
        {
            return connection.Square.DotsToHit.Contains(_entity.ID);

        }
        return false;

    }
}