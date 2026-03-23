using UnityEngine;
public class Replaceable : ModelBase, IReplaceable
{
    public Dot NewDot { get; set; }
    public void Replace()
    {
        if (NewDot == null) return;
        Debug.Log($"[Replaceable] Replacing {_entity.ID} with {NewDot.ID}");
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return;        
        
        boardService.BoardPresenter.ReplaceDot((Dot)_entity, NewDot);
        return;
        

    }
    public bool ShouldReplace()
    {
        if(_entity.TryGetModel(out Hittable hittable))
        {
            return hittable.ShouldClear();
        }
        if(_entity.TryGetModel(out Clearable clearable))
        {
            return clearable.ShouldClear();
        }
        return false;
    }
    public Replaceable(BoardEntity entity, Dot newDot) : base(entity)
    {
        NewDot = newDot;
    }
}