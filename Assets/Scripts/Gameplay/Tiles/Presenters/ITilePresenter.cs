using System;

public interface ITilePresenter
{
    TileModel Model { get; }
    TileView View { get; }
    event Action<ITilePresenter> OnTileRemoved;
    event Action<ITilePresenter> OnTileSpawned;
}