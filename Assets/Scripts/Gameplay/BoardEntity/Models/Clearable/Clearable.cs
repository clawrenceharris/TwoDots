using System;

/// <summary>
/// A model associated with a board entity that can be cleared or removed.
/// </summary>
public class Clearable : ModelBase, IClearable
{
    
    public IRule ClearRule { get; set; }
    public Hittable Hittable { get; set; }
    
    public Clearable(BoardEntity entity, Hittable hittable) : base(entity)
    {
        Hittable = hittable;
    }
    public Clearable(BoardEntity entity, IRule clearRule) : base(entity)
    {
        ClearRule = clearRule;
    }
    public Clearable(BoardEntity entity) : base(entity)
    {
        
    }
    public Clearable(BoardEntity entity, IRule clearRule, Hittable hittable) : base(entity)
    {
        ClearRule = clearRule;
        Hittable = hittable;
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
        return;
    }
}