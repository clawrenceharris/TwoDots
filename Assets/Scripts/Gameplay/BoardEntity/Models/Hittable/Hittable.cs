/// <summary>
/// A model associated with a board entity that can be hit.
/// </summary>
public class Hittable : ModelBase, IHittable
{
    /// <summary>The number of times the hittable has been hit.</summary>
    public int HitCount { get; set; }
    /// <summary>The maximum number of times the hittable can be hit.</summary>
    public int HitMax { get; set; }
    public virtual IRule HitRule { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Hittable"/> class.
    /// </summary>
    /// <param name="entity">The board entity</param>
    /// <param name="hitMax">The maximum times the model can be hit. Defaults to 1</param>
    /// <param name="hitCount">The number of times the model has been hit. Defaults to 0</param>
    public Hittable(BoardEntity entity, IRule hitRule = null, int hitMax = 1, int hitCount = 0) : base(entity)
    {
        HitCount = hitCount;
        HitMax = hitMax;
        HitRule = hitRule;
    }
   
    public virtual bool ShouldHit()
    {
        if (!ServiceProvider.Instance.TryGetService<ConnectionService>(out var connectionService)) return false;
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return false;
        
        return HitRule.CanHit(boardService.BoardPresenter, connectionService.ActiveConnection, _entity.ID);
        
    }
    public virtual bool ShouldClearAfterHit()
    {
        var tempHitCount = HitCount; // Create a temporary variable to store the hit count
        tempHitCount++; // Increment the temporary hit count to simulate a hit
        if (tempHitCount >= HitMax) // Check if the hit count is greater than or equal to the hit max
        {
            return true;
        }
        return false;
    }
    public virtual bool ShouldClear()
    {
        return HitCount >= HitMax;
    }
    public virtual void Hit()
    {
        HitCount++;
    }

}