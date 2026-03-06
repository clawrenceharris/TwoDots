using DG.Tweening;


/// <summary>
/// Interface for a presenter of a dot that can be hit.
/// </summary>
public interface IHittableDotPresenter  : IPresenter
{
    IClearableDotPresenter Clearable { get; }

    Sequence Hit();
}