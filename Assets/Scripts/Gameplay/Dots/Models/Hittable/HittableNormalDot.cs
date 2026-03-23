using UnityEngine;

public class HittableNormalDot : Hittable
{
    public HittableNormalDot(BoardEntity entity, int hitMax = 1, int hitCount = 0) : base(entity, new ConnectionRule(), hitMax, hitCount)
    {
        entity.AddModel(new Clearable(entity, this));
    }

   
   

}