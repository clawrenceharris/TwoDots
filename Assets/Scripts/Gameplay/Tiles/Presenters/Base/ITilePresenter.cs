using System;
using DG.Tweening;

public interface ITilePresenter
{
    Tile Tile { get; }
    TileView TileView { get; }

    event Action<ITilePresenter> OnTileRemoved;
    event Action<ITilePresenter> OnTileSpawned;
    Sequence Remove();
    Sequence Spawn();
}