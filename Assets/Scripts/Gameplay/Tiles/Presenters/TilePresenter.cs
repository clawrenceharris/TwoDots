using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Base class for all tile presenters. Handles dropping, spawning, 
/// and clearing and other tile management as it relates to the tile's presentation on the board.
/// </summary>
public class TilePresenter : ITilePresenter
{
    protected readonly TileView _view;
    private readonly ISkinResolver<Tile> _skinResolver;
    private readonly ISkinApplier<TileView> _skinApplier;
    protected readonly Tile _tile;
    public Tile Tile => _tile;
    public TileView View => _view;
    protected IBoardPresenter _board;

    public event Action<ITilePresenter> OnTileSpawned;
    public event Action<ITilePresenter> OnTileRemoved;

    private readonly Dictionary<Type, IPresenter> _presenters = new();

    public TilePresenter(Tile tile, TileView view)
    {
        _skinResolver = new TileSkinResolver();
        _skinApplier = new TileSkinApplier();
        _tile = tile;
        _view = view;
    }

    public void Initialize(IBoardPresenter board)
    {
        _view.Init(_tile);
        _board = board;
        RefreshSkin();
    }

    public Sequence Spawn()
    {
        return null;
    }


    public Sequence Remove()
    {
        return null;
    }
   


    private void RefreshSkin()
    {
        if (_view == null || _tile == null) return;
        var skin = _skinResolver.ResolveSkin(_tile);
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