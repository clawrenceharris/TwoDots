
/// <summary>
/// Presenter for the lotus dot. Not used yet but may be need in the future
/// </summary>
public class LotusDotPresenter : DotPresenter, IHittableDotPresenter
{
    public LotusDotPresenter(Dot dot, DotView view) : base(dot, view)
    {
    }

    public void Hit()
    {
        throw new System.NotImplementedException();
    }
}