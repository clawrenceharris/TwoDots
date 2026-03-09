using System;
using DG.Tweening;

public interface ITilePresenter
{
    Tile Tile { get; }
    TileView View { get; }

    void Initialize(IBoardPresenter board);

    event Action<ITilePresenter> OnTileRemoved;
    event Action<ITilePresenter> OnTileSpawned;
    Sequence Remove();
    Sequence Spawn();
}