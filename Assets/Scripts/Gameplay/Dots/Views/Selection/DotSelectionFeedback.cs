using DG.Tweening;
using UnityEngine;
using System;

public sealed class DotSelectionFeedback : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _pulseRenderer;
    [SerializeField] private SpriteRenderer _fillRenderer;
    [SerializeField] private float _pulseDuration = 0.65f;
    [SerializeField] private float _fillDuration = 0.2f;
    [SerializeField] private float _pulseStartScale = 0.8f;
    [SerializeField] private float _pulseEndScale = 2.5f;
    [SerializeField] private AnimationCurve _fillEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve _pulseEase = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float _pulseStartAlpha = 0.8f;
    [SerializeField] private float _fillEndScale = 1f;
    [SerializeField] private float _fillStartScale = 0.2f;


    private Sequence _selectionSequence;
    private Sequence _deselectionSequence;
    private bool _isSelected;

    private void KillSequences()
    {
        _selectionSequence?.Kill();
        _selectionSequence = null;
        _deselectionSequence?.Kill();
        _deselectionSequence = null;
    }

       public void PlaySelectionAnimation(Color color)
    {
        if (_pulseRenderer == null) return;

        // If this is the first time selecting, play pulse + fill
        if (!_isSelected)
        {
            KillSequences();
            StartPulse(color).Join(PlayFillAnimation(color));
            
            
        }
        else
        {
            // Already selected: only replay pulse, do not restart fill
            StartPulse(color);
        }
    }

    private Sequence StartPulse(Color color)
    {
        _pulseRenderer.color = color;
        _pulseRenderer.transform.localScale = Vector3.one * _pulseStartScale;
        var c = _pulseRenderer.color;
        c.a = _pulseStartAlpha;
        _pulseRenderer.color = c;
        _pulseRenderer.gameObject.SetActive(true);

        _selectionSequence?.Kill();
        _selectionSequence = DOTween.Sequence();
        _selectionSequence.Append(
            _pulseRenderer.transform
                .DOScale(Vector3.one * _pulseEndScale, _pulseDuration)
                .SetEase(_fillEase)
        );
        _selectionSequence.Join(
            _pulseRenderer
                .DOFade(0f, _pulseDuration)
                .SetEase(_pulseEase)
        );
        _selectionSequence.OnComplete(() =>
        {
            _pulseRenderer.gameObject.SetActive(false);
        });
        return _selectionSequence;
    }

    public Tween PlayFillAnimation(Color color)
    {
        if (_fillRenderer == null || _isSelected) return null;

        _isSelected = true;
        _fillRenderer.color = color;
        _fillRenderer.transform.localScale = Vector3.one * _fillStartScale;
        _fillRenderer.gameObject.SetActive(true);
        Tween fillTween = _fillRenderer.transform
            .DOScale(Vector3.one * _fillEndScale, _fillDuration)
            .SetEase(_fillEase);
        return fillTween;
    }

    public void PlayDeselectionAnimation()
    {
        if (_fillRenderer == null) return;
        _deselectionSequence?.Kill();

        _deselectionSequence = DOTween.Sequence();
        _deselectionSequence.Append(
            _fillRenderer.transform
                .DOScale(Vector3.zero, _fillDuration)
                .SetEase(_fillEase)
        );
        _deselectionSequence.OnComplete(() =>
        {
            _fillRenderer.gameObject.SetActive(false);
            _isSelected = false;
        });
    }

    public void SetFillColor(Color color)
    {
        _fillRenderer.color = color;
    }
}