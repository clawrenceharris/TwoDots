using System;

public class ClearableDot : DotModel, IClearableDot
{
    public Func<bool> ShouldClear { get; set; } = () => false;

    public ClearableDot(Dot dot) : base(dot)
    {
    }
    public ClearableDot(Dot dot, Func<bool> shouldClear) : base(dot)
    {
        ShouldClear = shouldClear;
    }

    public void Clear()
    {
        return;
    }
}