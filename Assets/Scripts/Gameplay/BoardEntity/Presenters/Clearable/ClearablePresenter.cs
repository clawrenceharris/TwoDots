using DG.Tweening;
using UnityEngine;

public class ClearablePresenter : EntityPresenter, IClearablePresenter
{
    

    public ClearablePresenter(BoardEntity entity, EntityView view) : base(entity, view)
    {
    }

    public virtual Sequence Clear()
    {
       return DOTween.Sequence().Append(View.transform.DOScale(Vector3.zero, 0.3f));
    }
}