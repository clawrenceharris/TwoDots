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
    
    public IEnumerator DoLineAnimation(IDotPresenter dot, Action callback = null)
    {

        float duration = 0.4f;
        Vector3 startPos = transform.position;
        Vector3 targetPosition = GridUtility.GridToWorld(dot.Dot.GridPosition);

        float angle = Vector2.SignedAngle(Vector2.right, targetPosition - startPos);
        float distance = Vector2.Distance(startPos, targetPosition);
        distance -= dot.View.transform.localScale.x / 2 + dot.View.transform.localScale.x / 2;

        ConnectorLineView line = new GameObject("Line").AddComponent<ConnectorLineView>();

        line.enabled = false;
        line.SetColor(ColorSchemeService.CurrentColorScheme.bombLight);
        line.GetComponent<LineRenderer>().sortingLayerName = "Bomb";
        line.GetComponent<LineRenderer>().sortingOrder = 100;
        line.transform.SetPositionAndRotation(startPos, Quaternion.Euler(0, 0, angle));

        line.transform.localScale = new Vector3(0, 0.2f);
        line.transform.parent = dot.View.transform;

        line.transform.DOScale(new Vector3(distance, 0.2f), duration / 2).WaitForCompletion();

        callback?.Invoke();
        Vector3 position = targetPosition - (targetPosition - startPos).normalized * distance;

        line.transform.DOMove(position, duration);


        line.transform.DOScale(new Vector3(distance, 0.1f), duration / 3);


        line.transform.DOMove(targetPosition, duration / 3);

        line.transform.DOScale(new Vector3(0, 0.1f), duration / 3);
        yield return new WaitForSeconds(duration);
        Destroy(line.gameObject);
    }
}