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
    private readonly ISkinResolver<Dot> _skinResolver;
    private readonly ISkinApplier<DotView> _skinApplier;
    protected readonly Dot _dot;
    public Dot Dot => _dot;
    public DotView View => _view;
    protected IBoardPresenter _board;

    public event Action<string> OnDotSpawned;

    public event Action<string> OnDotDropped;
    private readonly Dictionary<Type, IPresenter> _presenters = new();

    public DotPresenter(Dot dot, DotView view)
    {
        _skinResolver = new DotSkinResolver();
        _skinApplier = new DotSkinApplier();
        _dot = dot;
        _view = view;

        PrepareForDrop();
    }

    public void Initialize(IBoardPresenter board)
    {
        _view.Init(_dot);
        _board = board;
        RefreshSkin();
    }

    public Sequence Spawn()
    {
        return DOTween.Sequence().Append(_view.transform.DOScale(Vector3.one, 0.3f)).OnComplete(() =>
        {
            OnDotSpawned?.Invoke(_dot.ID);
        });
    }



    public void Drop(int targetRow)
    {

        var endPos = GridUtility.GridToWorld(_dot.GridPosition);
        _view.transform.DOMoveY(targetRow * BoardView.TileSize, 0.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _view.transform.position = endPos;
            OnDotDropped?.Invoke(_dot.ID);
        });
    }
    public void PrepareForDrop()
    {
        var startPos = GridUtility.GridToWorld(_dot.GridPosition);
        _view.transform.position = new Vector3(startPos.x, (Camera.main.orthographicSize * 2) + startPos.y, 0);
    }



    private void RefreshSkin()
    {
        if (_view == null || _dot == null) return;
        var skin = _skinResolver.ResolveSkin(_dot);
        _skinApplier.Apply(_view, skin);
    }

    public void AddPresenter<T>(T presenter) where T : class, IPresenter
    {
        _presenters.Add(typeof(T), presenter);
    }

    public void RemovePresenter<T>() where T : class, IPresenter
    {
        _presenters.Remove(typeof(T));
    }
    public T GetPresenter<T>() where T : class, IPresenter
    {
        if (TryGetPresenter(out T presenter))
        {
            return presenter;
        }
        return null;
    }


    public bool TryGetPresenter<T>(out T presenter) where T : class, IPresenter
    {

        if (_presenters.TryGetValue(typeof(T), out IPresenter tPresenter))
        {
            presenter = tPresenter as T;
            return true;
        }
        else
        {
            foreach (var kvp in _presenters)
            {
                if (kvp.Value is T tPresenterValue)
                {
                    presenter = tPresenterValue;
                    return true;
                }
            }
        }
        presenter = null;
        return false;
    }
}