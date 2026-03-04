/// <summary>
/// Presenter for a dot that can be hit.
/// </summary>
public class HittableDotPresenter : DotPresenter, IHittableDotPresenter
{
    public HittableDotPresenter(Dot dot, DotView view) : base(dot, view)
    {
    }

    public void Hit()
    {
        throw new System.NotImplementedException();
    }
}