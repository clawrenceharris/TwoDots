using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NestingPreviewPresenter : EntityPresenter, IPreviewablePresenter
{
    private Tween _shakeTween;
    private Tween _pulseTween;

    public NestingPreviewPresenter(Dot dot, DotView view) : base(dot, view)
    {
    }

    public void ApplyPreviewSignals(PreviewSignal activeSignals, List<PreviewTransition> transitions)
    {
        bool shouldShake = (activeSignals & PreviewSignal.Shake) != 0;
        bool shouldPulse = (activeSignals & PreviewSignal.Pulse) != 0;

        if (shouldShake) StartShake();
        else StopShake();

        if (shouldPulse) StartPulse();
        else StopPulse();
    }

    public void ResetPreview()
    {
        StopShake();
        StopPulse();
        if (View != null)
        {
            View.transform.localScale = Vector3.one;
        }
    }

    private void StartShake()
    {
        if (_shakeTween != null && _shakeTween.IsActive()) return;
        _shakeTween = View.transform.DOShakePosition(0.2f, strength: 0.09f, vibrato: 8, randomness: 90f, snapping: false, fadeOut: true)
            .SetLoops(-1, LoopType.Restart);
    }

    private void StopShake()
    {
        if (_shakeTween == null) return;
        _shakeTween.Kill();
        _shakeTween = null;
    }

    private void StartPulse()
    {
        if (_pulseTween != null && _pulseTween.IsActive()) return;
        _pulseTween = View.transform.DOScale(1.08f, 0.3f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopPulse()
    {
        if (_pulseTween == null) return;
        _pulseTween.Kill();
        _pulseTween = null;
        View.transform.localScale = Vector3.one;
    }
}
