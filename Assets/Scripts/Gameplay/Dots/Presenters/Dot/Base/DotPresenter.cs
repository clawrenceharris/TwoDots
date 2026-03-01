using System;
using System.Collections.Generic;
using DG.Tweening;
using Dots.Utilities;
using UnityEngine;
/// <summary>
/// Base class for all dot presenters. Handles dropping, spawning, 
/// and clearing and other dot management as it relates to the dot's presentation on the board.
/// </summary>
public class DotPresenter : IDotPresenter
{
    protected readonly DotView _view;
    private readonly ISkinResolver _skinResolver;
    private readonly IDotSkinApplier _skinApplier;
    protected readonly Dot _dot;
    public Dot Dot => _dot;
    public DotView View => _view;
    public event Action<IDotPresenter> OnDotCleared;
    public event Action<IDotPresenter> OnDotDropped;
    private readonly Dictionary<Type, IDotPresenter> _presenters = new();

    public DotPresenter(
        Dot dot,
        DotView view)
    {
        _skinResolver = new SkinResolver();
        _skinApplier = new DotSkinApplier();
        _dot = dot;
        _view = view;
        _view.Init(dot);
        RefreshSkin();
        PrepareForDrop();
    }

    public Sequence Clear()
    {
        return DOTween.Sequence().Append(_view.transform.DOScale(Vector3.zero, 0.3f)).OnComplete(() =>
        {
            OnDotCleared?.Invoke(this);

        });
    }

    public Sequence Spawn()
    {
        throw new System.NotImplementedException();
    }


   
    public void Drop(int targetRow)
    {
        
        var endPos = GridUtility.GridToWorld(_dot.GridPosition);
        _view.transform.DOMoveY(targetRow * BoardView.TileSize, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _view.transform.position = endPos;
            OnDotDropped?.Invoke(this);
        });
    }
    public void PrepareForDrop()
    {
        var worldPos = GridUtility.GridToWorld(_dot.GridPosition);
        _view.transform.position = new Vector3(worldPos.x, (Camera.main.orthographicSize * 2 )+ BoardView.TileSize, 0);
    }



    private void RefreshSkin()
    {
        if (_view == null || _dot == null) return;
        var skin = _skinResolver.ResolveDotSkin(_dot);
        _skinApplier.Apply(_view, skin);
    }

    public void AddPresenter<T>(T presenter) where T : DotPresenter
    {
        _presenters.Add(typeof(T), presenter);
    }

    public void RemovePresenter<T>() where T : DotPresenter
    {
        _presenters.Remove(typeof(T));
    }
    public T GetPresenter<T>() where T : DotPresenter
    {
        if (_presenters.TryGetValue(typeof(T), out var presenter))
        {
            return presenter as T;
        }
        Debug.LogWarning($"Presenter {typeof(T)} not found");
        return null;
    }

    public bool TryGetPresenter<T>(out T presenter) where T : DotPresenter
    {
        presenter = GetPresenter<T>();
        return presenter != null;
    }
}