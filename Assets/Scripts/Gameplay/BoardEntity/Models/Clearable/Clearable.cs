using System;

/// <summary>
/// A model associated with a board entity that can be cleared or removed.
/// </summary>
public class Clearable : ModelBase, IClearable
{
    
    public IRule ClearRule { get; set; }
    public Hittable Hittable { get; set; }
    public Action<IClearable> OnClear { get; set; }
    public Clearable(BoardEntity entity, Hittable hittable, Action<IClearable> onClear = null) : base(entity)
    {
        Hittable = hittable;
        OnClear = onClear;
    }
    public Clearable(BoardEntity entity, IRule clearRule, Action<IClearable> onClear = null) : base(entity)
    {
        ClearRule = clearRule;
        OnClear = onClear;
    }
    public Clearable(BoardEntity entity, Action<IClearable> onClear = null) : base(entity)
    {
        OnClear = onClear;
    }
    public Clearable(BoardEntity entity, IRule clearRule, Hittable hittable, Action<IClearable> onClear = null) : base(entity)
    {
        ClearRule = clearRule;
        Hittable = hittable;
        OnClear = onClear;
    }
    
    public bool ShouldClear()
    {
        if (Hittable != null)
        {           
            return Hittable.ShouldClear();  
        }
        if(ClearRule != null)
        {
            if (!ServiceProvider.Instance.TryGetService<ConnectionService>(out var connectionService)) return false;
            if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return false;
            return ClearRule.CanHit(boardService.BoardPresenter, connectionService.ActiveConnection, _entity.ID);
        }
        // If no clear rule or hittable rule, default to true
        return true;
    }
    public void Clear()
    {
        OnClear?.Invoke(this);
    }
}