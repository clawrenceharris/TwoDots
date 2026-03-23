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
    
    public Sequence DoLineAnimation(IHittablePresenter hittable)
    {
        var sequence = DOTween.Sequence();
        float duration = 0.3f;
        Vector3 startPos =transform.position;
        Vector3 targetPosition = GridUtility.GridToWorld(hittable.Entity.GridPosition);
        float angle = Vector2.SignedAngle(Vector2.right, targetPosition - startPos);
        float distance = Vector2.Distance(startPos, targetPosition);
        distance -= hittable.View.transform.localScale.x / 2 + hittable.View.transform.localScale.x / 2;

        GameObject line = Instantiate(_visuals.BombLine, startPos, Quaternion.Euler(0, 0, angle));
        var renderer = line.GetComponent<LineRenderer>();
        renderer.material.color = ServiceProvider.Instance.GetService<ColorSchemeService>().CurrentColorScheme.blank;
        renderer.sortingOrder = -1;
        renderer.sortingLayerName = "Dots";
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
                // Calculate the direction from the center of the hittable dot to the incoming line
                Vector3 direction = (lineStart - targetPosition).normalized;
                // Estimate the edge position of the hittable dot in that direction
                float radius = hittable.View.transform.localScale.x / 2f;
                Vector3 edgePosition = targetPosition + direction * radius;

                renderer.SetPosition(1, Vector3.Lerp(lineStart, edgePosition, t));
                
                
            },
            1f,
            duration * 0.5f
        ));
        sequence.AppendCallback(() =>
        {
            // Change to HitMaterial as soon as the line reaches the dot's edge
            if (hittable.View.Renderer != null)
            {
                Debug.Log("Setting color to blank");
                hittable.View.Renderer.HitMaterial.color = ServiceProvider.Instance.GetService<ColorSchemeService>().CurrentColorScheme.blank;
                hittable.View.Renderer.SetMaterial(hittable.View.Renderer.HitMaterial);
            }
            
        });
        sequence.AppendCallback(() => {
            
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
            duration * 0.5f
        ));

        sequence.AppendInterval(duration * 0.5f).AppendCallback(() => {
            if(hittable.View.Renderer != null)
            {
                hittable.View.Renderer.SetMaterial(hittable.View.Renderer.DefaultMaterial);
            }
        });
        sequence.OnComplete(() => Destroy(line.gameObject));
        return sequence;
    }
}