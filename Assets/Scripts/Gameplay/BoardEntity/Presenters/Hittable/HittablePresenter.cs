using DG.Tweening;
using UnityEngine;


/// <summary>
/// Presenter for a dot that can be hit.
/// </summary>
public class HittablePresenter: EntityPresenter,  IHittablePresenter
{
   
    public HittablePresenter(BoardEntity entity, EntityView view) : base(entity, view)
    {
    }

    public virtual Sequence Hit()
    {
        return null;
    }
}

