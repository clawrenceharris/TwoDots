using System;

/// <summary>
/// A model associated with a board entity that can be cleared or removed.
/// </summary>
public class Clearable : ModelBase, IClearable
{
    /// <summary>
    /// A function that returns true if the clearable should be cleared.
    /// </summary>
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