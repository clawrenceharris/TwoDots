using DG.Tweening;
using UnityEngine;

public class ClearablePresenter : EntityPresenter, IClearablePresenter
{
    

    public ClearablePresenter(BoardEntity entity, EntityView view) : base(entity, view)
    {
    }

    public Sequence Clear()
    {
       return DOTween.Sequence().Append(GetView().transform.DOScale(Vector3.zero, 0.3f));
    }
}