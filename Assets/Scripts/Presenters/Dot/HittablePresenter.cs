public class HittablePresenter : DotPresenter, IHittableDotPresenter
{
    public HittablePresenter(Dot dot, DotView view) : base(dot, view)
    {
    }

    public void Hit()
    {
        throw new System.NotImplementedException();
    }
}