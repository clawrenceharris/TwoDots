using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BeetlePreviewPresenter : EntityPresenter, IPreviewablePresenter
{
    private Tween _shakeTween;
    private Tween _wingFlapTween;

    public BeetlePreviewPresenter(Dot dot, DotView view) : base(dot, view)
    {
    }

    public void ApplyPreviewSignals(PreviewSignal activeSignals, List<PreviewTransition> transitions)
    {
        bool shouldShake = (activeSignals & PreviewSignal.Shake) != 0;
        bool shouldFlap = (activeSignals & PreviewSignal.WingFlap) != 0;

        if (shouldShake) StartShake();
        else StopShake();

        if (shouldFlap) StartWingFlap();
        else StopWingFlap();
    }

    public void ResetPreview()
    {
        StopShake();
        StopWingFlap();
        if (View != null)
        {
            View.transform.localRotation = Quaternion.identity;
            View.transform.localScale = Vector3.one;
        }
    }

    private void StartShake()
    {
        if (_shakeTween != null && _shakeTween.IsActive()) return;
        _shakeTween = View.transform.DOShakePosition(0.2f, strength: 0.08f, vibrato: 8, randomness: 90f, snapping: false, fadeOut: true)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopShake()
    {
        if (_shakeTween == null) return;
        _shakeTween.Kill();
        _shakeTween = null;
    }

    private void StartWingFlap()
    {
        if (_wingFlapTween != null && _wingFlapTween.IsActive()) return;
        _wingFlapTween = View.transform.DOLocalRotate(new Vector3(0f, 0f, 8f), 0.18f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopWingFlap()
    {
        if (_wingFlapTween == null) return;
        _wingFlapTween.Kill();
        _wingFlapTween = null;
        View.transform.localRotation = Quaternion.identity;
    }
}
