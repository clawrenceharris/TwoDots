public class HittableDot : DotModel, IHittableDot
{
    public int HitCount { get; set; }
    public int HitMax { get; set; }
    public IClearableDot Clearable { get; set; }
    public HittableDot( Dot dot, int hitMax, int hitCount = 0) : base(dot)
    {
        HitCount = hitCount;
        HitMax = hitMax;
    }
    public HittableDot(Dot dot, ClearableDot clearable, int hitMax, int hitCount = 0) : base(dot)
    {
        Clearable = clearable;
        HitCount = hitCount;
        HitMax = hitMax;
        Clearable.ShouldClear = ShouldClear;
    }
    public bool ShouldClear()
    {
        return HitCount >= HitMax;
    }
    public bool ShouldHit()
    {
        return HitCount < HitMax;
    }

    public void Hit()
    {
        HitCount++;
    }

}