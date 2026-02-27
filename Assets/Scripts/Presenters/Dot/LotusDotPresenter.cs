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