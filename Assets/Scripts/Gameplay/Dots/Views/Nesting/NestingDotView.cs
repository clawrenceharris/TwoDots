using DG.Tweening;
using UnityEngine;

[UnityEngine.RequireComponent(typeof(NestingDotVisuals))]
public class NestingDotView : DotView
{
    private NestingDotVisuals _visuals;
    public override void Init(Dot dot)
    {
        _visuals = GetVisuals<NestingDotVisuals>();
        base.Init(dot);
    }

    public Sequence PlayHitAnimation()
    {
        
        var sequence = DOTween.Sequence();
        float duration = 0.9f;
        
        _visuals.BottomCap.enabled = true;
        _visuals.TopCap.enabled = true;
        _visuals.BottomCap.color = Renderer.BaseRenderer.color;
        _visuals.TopCap.color = Renderer.BaseRenderer.color;
        
        sequence.Append(_visuals.BottomCap.transform.DOMoveY(transform.position.y - BoardView.TileSize, duration));
        sequence.Join(_visuals.TopCap.transform.DOMoveY(transform.position.y + BoardView.TileSize, duration));
        sequence.Join(_visuals.BottomCap.DOFade(0, duration));
        sequence.Join(_visuals.TopCap.DOFade(0, duration));
        sequence.OnComplete(() =>
        {
            _visuals.BottomCap.enabled = false;
            _visuals.BottomCap.transform.localPosition = Vector3.zero;
            _visuals.TopCap.enabled = false;
            _visuals.TopCap.transform.localPosition = Vector3.zero;
            
        });
        return sequence;
    }


}