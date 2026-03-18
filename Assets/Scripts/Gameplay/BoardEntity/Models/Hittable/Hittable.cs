/// <summary>
/// A model associated with a board entity that can be hit.
/// </summary>
public class Hittable : ModelBase, IHittable
{
    /// <summary>The number of times the hittable has been hit.</summary>
    public int HitCount { get; set; }
    /// <summary>The maximum number of times the hittable can be hit.</summary>
    public int HitMax { get; set; }
    /// <summary>A model that can be cleared after the hittable has been hit.</summary>
    public IClearable Clearable { get; set; }
    public virtual IHitRule HitRule => new NormalHitRule();

    /// <summary>
    /// Initializes a new instance of the <see cref="Hittable"/> class.
    /// </summary>
    /// <param name="entity">The board entity</param>
    /// <param name="hitMax">The maximum times the model can be hit. Defaults to 1</param>
    /// <param name="hitCount">The number of times the model has been hit. Defaults to 0</param>
    public Hittable(BoardEntity entity, int hitMax = 1, int hitCount = 0) : base(entity)
    {
        HitCount = hitCount;
        HitMax = hitMax;
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="Hittable"/> class.
    /// This constructor also adds the given clearable model to the entity.
    /// </summary>
    /// <param name="entity">The board entity</param>
    /// <param name="clearable">The clearable model</param>
    /// <param name="hitMax">The maximum times the model can be hit. Defaults to 1</param>
    /// <param name="hitCount">The number of times the model has been hit. Defaults to 0</param>
    public Hittable(BoardEntity entity, Clearable clearable, int hitMax, int hitCount = 0) : base(entity)
    {
        Clearable = clearable;
        Clearable.ShouldClear = ShouldClear;
        HitCount = hitCount;
        HitMax = hitMax;
        entity.AddModel(clearable);
    }

    public virtual bool ShouldHit()
    {
        return false;
    }
    public virtual bool ShouldClearAfterHit()
    {
        if (Clearable == null) return false;
        var tempHitCount = HitCount; // Create a temporary variable to store the hit count
        tempHitCount++; // Increment the temporary hit count to simulate a hit
        if (tempHitCount >= HitMax) // Check if the hit count is greater than or equal to the hit max
        {
            return true;
        }
        return false;
    }

    protected virtual bool ShouldClear()
    {
        return HitCount >= HitMax;
    }
    public virtual void Hit()
    {
        HitCount++;
    }

}