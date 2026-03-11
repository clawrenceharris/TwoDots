using System.Collections.Generic;

public enum HitConditionType
{
    Connection,
    Square,
    AdjacentToConnection,
    AdjacentToSquare,
}

public class Hittable : ModelBase, IHittable
{
    public List<HitConditionType> Conditions { get; set; }
    public int HitCount { get; set; }
    public int HitMax { get; set; }
    public IClearable Clearable { get; set; }
    public IHitRule HitRule { get; set; }
    public Hittable(BoardEntity entity, int hitMax, List<HitConditionType> conditions, int hitCount = 0) : base(entity)
    {
        HitCount = hitCount;
        HitMax = hitMax;
        Conditions = conditions;
    }
    public Hittable(BoardEntity entity, Clearable clearable,int hitMax,  List<HitConditionType> conditions,  int hitCount = 0) : base(entity)
    {
        Clearable = clearable;
        Clearable.ShouldClear = ShouldClear;

        HitCount = hitCount;
        HitMax = hitMax;
        Conditions = conditions;
        entity.AddModel(clearable);
    }
    public virtual bool ShouldClear()
    {
        return HitCount >= HitMax;
    }
    public virtual bool ShouldHit()
    {
        return HitCount < HitMax;
    }

    public virtual void Hit()
    {
        HitCount++;
    }

}