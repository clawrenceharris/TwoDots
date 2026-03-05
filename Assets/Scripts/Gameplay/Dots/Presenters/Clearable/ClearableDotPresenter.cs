using DG.Tweening;
using UnityEngine;

public class ClearableDotPresenter : BasePresenter, IClearableDotPresenter
{
    public ClearableDotPresenter(IDotPresenter dot, IBoardPresenter board)
    : base(dot, board)
    {
    }

    public Sequence Clear()
    {
       return DOTween.Sequence().Append(_dot.View.transform.DOScale(Vector3.zero, 0.3f));
    }
}