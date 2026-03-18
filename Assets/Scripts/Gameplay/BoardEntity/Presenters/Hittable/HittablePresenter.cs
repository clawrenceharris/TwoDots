using DG.Tweening;
using UnityEngine;


/// <summary>
/// Presenter for a dot that can be hit.
/// </summary>
public class HittablePresenter: EntityPresenter,  IHittablePresenter
{
   
    public IClearablePresenter Clearable { get; private set; }
    public HittablePresenter(BoardEntity entity, EntityView view, IClearablePresenter clearable = null) : base(entity, view)
    {
        Clearable = clearable;  
        // AddPresenter(Clearable);
    }

    public virtual Sequence Hit()
    {
        return null;
    }
}

