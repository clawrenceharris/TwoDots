using DG.Tweening;

public class ClearableNestingPresenter : ClearablePresenter
{
    public ClearableNestingPresenter(Dot dot, DotView view) : base(dot, view)
    {
    }

    public override Sequence Clear()
    {
        return null;
    }
}