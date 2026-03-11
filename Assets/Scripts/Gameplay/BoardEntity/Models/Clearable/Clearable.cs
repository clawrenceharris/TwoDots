using System;

public class Clearable : ModelBase, IClearable
{
    public Func<bool> ShouldClear { get; set; } = () => false;

    public Clearable(BoardEntity entity) : base(entity)
    {
    }
    public Clearable(BoardEntity entity, Func<bool> shouldClear) : base(entity)
    {
        ShouldClear = shouldClear;
    }

    public void Clear()
    {
        return;
    }
}