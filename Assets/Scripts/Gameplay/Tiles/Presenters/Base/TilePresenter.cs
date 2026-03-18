using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Base class for all tile presenters. Handles dropping, spawning, 
/// and clearing and other tile management as it relates to the tile's presentation on the board.
/// </summary>
public class TilePresenter : EntityPresenter, ITilePresenter
{
    private readonly ISkinResolver<Tile> _skinResolver;
    private readonly ISkinApplier<TileView> _skinApplier;
    public Tile Tile => _entity as Tile;
    public TileView TileView => _view as TileView;
    public event Action<ITilePresenter> OnTileSpawned;
    public event Action<ITilePresenter> OnTileRemoved;

    private readonly Dictionary<Type, IPresenter> _presenters = new();

    public TilePresenter(Tile tile, TileView view) : base(tile, view)
    {
        _skinResolver = new TileSkinResolver();
        _skinApplier = new TileSkinApplier();
      
    }

    public override void Initialize(IBoardPresenter board)
    {
        TileView.Init(Tile);
        _board = board;
        RefreshSkin();
        TileView.transform.localScale = Vector3.one * BoardView.TileSize;

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
        if (_view == null || Tile == null) return;
        var skin = _skinResolver.ResolveSkin(Tile);
        _skinApplier.Apply(TileView, skin);
    }

}