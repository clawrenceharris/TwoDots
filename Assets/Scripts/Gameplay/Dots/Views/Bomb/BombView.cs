using System;
using System.Collections;
using DG.Tweening;
using Dots.Utilities;
using UnityEngine;

[RequireComponent(typeof(BombVisuals))]
public class BombView : DotView
{
    private BombVisuals _visuals;
    public override void Init(Dot dot)
    {
        base.Init(dot);
        _visuals = GetComponent<BombVisuals>();
    }
    
    public Sequence DoLineAnimation(IHittablePresenter hittable, Action callback = null)
    {
        var sequence = DOTween.Sequence();
        float duration = 0.35f;
        Vector3 startPos = transform.position;
        Vector3 targetPosition = GridUtility.GridToWorld(hittable.GetEntity().GridPosition);
        float angle = Vector2.SignedAngle(Vector2.right, targetPosition - startPos);
        float distance = Vector2.Distance(startPos, targetPosition);
        distance -= hittable.GetView().transform.localScale.x / 2 + hittable.GetView().transform.localScale.x / 2;

        GameObject line = Instantiate(_visuals.BombLine, startPos, Quaternion.Euler(0, 0, angle));
        var renderer = line.GetComponent<LineRenderer>();
        renderer.material.color = ServiceProvider.Instance.GetService<ColorSchemeService>().CurrentColorScheme.blank;
        renderer.sortingLayerName = "Bomb";
        renderer.sortingOrder = 100;

        renderer.startWidth = 0.2f;
        renderer.endWidth = 0.2f;
        renderer.positionCount = 2;

        // Set line to start small "behind" the bomb view (opposite direction to target)
        float startLength = 0.3f; // tweak for visual effect
        Vector3 forward = (targetPosition - startPos).normalized;
        Vector3 startOffset = -forward * startLength;
        Vector3 lineStart = startPos + startOffset;
        renderer.SetPosition(0, lineStart);
        renderer.SetPosition(1, lineStart);

        // As the line animates, the head moves to the target, then the tail follows
        // Phase 1: Head extends from start to target, tail remains at lineStart
        sequence.Append(DOTween.To(
            () => 0f,
            t =>
            {
                renderer.SetPosition(0, lineStart);
                renderer.SetPosition(1, Vector3.Lerp(lineStart, targetPosition, t));
                if (Vector3.Distance(renderer.GetPosition(0), hittable.GetView().transform.position - (hittable.GetView().transform.localScale / 2)) < 0.01f)
                {
                    if(hittable.GetView().Renderer != null)
                    {
                        hittable.GetView().Renderer.SetColor(ServiceProvider.Instance.GetService<ColorSchemeService>().CurrentColorScheme.blank);
                    }
                }
            },
            1f,
            duration * 0.5f
        )).AppendCallback(() => {
            if(hittable.GetView().Renderer != null)
            {
                hittable.GetView().Renderer.SetColor(ServiceProvider.Instance.GetService<ColorSchemeService>().ToDotColor(hittable.GetEntity()));
            }
        });

        // Phase 2: Tail follows to target, so line 'collapses' into target
        sequence.Join(DOTween.To(
            () => 0f,
            t =>
            {
                renderer.SetPosition(0, Vector3.Lerp(lineStart, targetPosition, t));
                renderer.startWidth = Mathf.Lerp(0.2f, 0.1f, t);
                renderer.endWidth = Mathf.Lerp(0.2f, 0.1f, t);

            },
            1f,
            duration * 1.3f
        ));


        sequence.OnComplete(() => Destroy(line.gameObject));
        return sequence;
    }
}