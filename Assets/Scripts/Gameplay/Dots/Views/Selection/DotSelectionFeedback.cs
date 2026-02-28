using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public sealed class DotSelectionFeedback : MonoBehaviour, IModel
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
    public bool IsPlaying;



    private void Awake()
    {

        if (_pulseRenderer == null)
        {
            return;
        }
        _pulseRenderer.gameObject.SetActive(false);
    }

   

    public void ChangeFillColor(Color color)
    {
        _fillRenderer.color = color;
    }

    public void PlaySelectionAnimation(Color color)
    {


        if (_pulseRenderer == null) return;
        _pulseRenderer.color = color;
        _pulseRenderer.transform.localScale = Vector3.one * _pulseStartScale;

        // Ensure fully visible at start
        var c = _pulseRenderer.color;
        c.a = _pulseStartAlpha;
        _pulseRenderer.color = c;
        _pulseRenderer.gameObject.SetActive(true);


        var sequence = DOTween.Sequence();
        sequence.Append(_pulseRenderer.transform.DOScale(Vector3.one * _pulseEndScale, _pulseDuration).SetEase(_fillEase));
        sequence.Join(_pulseRenderer.DOFade(0f, _pulseDuration).SetEase(_pulseEase));
        sequence.JoinCallback(() => PlayFillAnimation(color));
        sequence.OnComplete(() =>
        {
            IsPlaying = false;
            _pulseRenderer.gameObject.SetActive(false);
        });

        
        
        
    }

    public void PlayFillAnimation(Color color)
    {
        

        if (_fillRenderer == null) return;
        
        IsPlaying = true;
        _fillRenderer.color = color;
        _fillRenderer.transform.localScale = Vector3.zero * _fillStartScale;
        _fillRenderer.gameObject.SetActive(true);
        _fillRenderer.transform.DOScale(Vector3.one * _fillEndScale, _fillDuration)
        .SetEase(_fillEase);
    }

    public void PlayDeselectionAnimation()
    {
        if (_pulseRenderer == null) return;
        _fillRenderer.transform.DOScale(Vector3.zero, _fillDuration)
        .SetEase(_fillEase).OnComplete(() =>
        {
            _fillRenderer.gameObject.SetActive(false);
        });
    }

    
}