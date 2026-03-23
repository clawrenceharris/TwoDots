using System;
using System.Collections.Generic;
using DG.Tweening;
using Dots.Utilities;
using UnityEngine;

/// <summary>
/// Base class for all dot presenters. Handles dropping, spawning,
///  and other dot management as it relates to the dot's presentation on the board.
/// </summary>
public class DotPresenter : EntityPresenter, IDotPresenter
{
    private readonly ISkinResolver<Dot> _skinResolver;
    private readonly ISkinApplier<DotView> _skinApplier;
    public Dot Dot => _entity as Dot;
    public DotView DotView => _view as DotView;
    public event Action<string> OnDotSpawned;

    public event Action<string> OnDotDropped;

    public DotPresenter(Dot dot, DotView view) : base(dot, view)
    {
        _skinResolver = new DotSkinResolver();
        _skinApplier = new DotSkinApplier();
        PrepareForDrop();
    }

    public override void Initialize()
    {
        DotView.Init(Dot);
        RefreshSkin();
    }

    public Sequence Spawn()
    {
        return DOTween.Sequence().Append(_view.transform.DOScale(Vector3.one, 0.3f)).OnComplete(() =>
        {
            OnDotSpawned?.Invoke(Dot.ID);
        });
    }



    public Sequence Drop(int targetRow)
    {

        var endPos = GridUtility.GridToWorld(Dot.GridPosition);
        return DOTween.Sequence().Append(_view.transform.DOMoveY(targetRow * BoardView.TileSize, 0.5f).SetEase(Ease.OutBounce)).OnComplete(() =>
        {
            _view.transform.position = endPos;
            OnDotDropped?.Invoke(Dot.ID);
        });
    }
    public void PrepareForDrop()
    {
        var startPos = GridUtility.GridToWorld(Dot.GridPosition);
        _view.transform.position = new Vector3(startPos.x, (Camera.main.orthographicSize * 2) + startPos.y, 0);
    }



    private void RefreshSkin()
    {
        if (_view == null || Dot == null) return;
        var skin = _skinResolver.ResolveSkin(Dot);
        if (skin.HasValue)
        {
            _skinApplier.Apply(DotView, skin.Value);
        }
    }

}