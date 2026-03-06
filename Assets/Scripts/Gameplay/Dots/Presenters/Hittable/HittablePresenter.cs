using DG.Tweening;
using UnityEngine;


/// <summary>
/// Presenter for a dot that can be hit.
/// </summary>
public class HittableDotPresenter : BasePresenter, IHittableDotPresenter
{
    public IClearableDotPresenter Clearable { get; private set; }
    public HittableDotPresenter(IDotPresenter dot, IBoardPresenter board, IClearableDotPresenter clearable = null)
    : base(dot, board)
    {
        Clearable = clearable;
    }

    public virtual Sequence Hit()
    {
        return null;
    }
}

