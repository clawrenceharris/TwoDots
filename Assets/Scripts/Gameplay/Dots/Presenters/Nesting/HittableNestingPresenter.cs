using DG.Tweening;
using UnityEngine;

public class HittableNestingPresenter : HittablePresenter
{
    private readonly NestingDotView NestingDotView;
    public HittableNestingPresenter(BoardEntity entity, EntityView view) : base(entity, view)
    {
        NestingDotView = (NestingDotView)view;
    }
    public override Sequence Hit()
    {
        var hittableModel = Entity.GetModel<Hittable>();
        
        if (hittableModel.HitCount == 1)
        {
            View.transform.localScale =  Vector3.one * 1.5f;
        }
        else if (hittableModel.HitCount == 2)
        {
            View.transform.localScale =  Vector3.one * 1f;
        }
        NestingDotView.PlayHitAnimation();
        return null;
    }
}