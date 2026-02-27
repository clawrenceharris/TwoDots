using System;
using UnityEngine;

public class TileFactory
{
    public static TileModel CreateTileModel(DotsObject data)
    {
        var type = data.GetProperty<TileType>(DotsObject.Property.Type);
        switch (type)
        {
            default:
                return new TileModel(new Vector2Int(data.Col, data.Row), type);
        }
    }

    internal static ITilePresenter CreateTilePresenter(TileModel tile, TileView view)
    {
        switch (tile.TileType)
        {
            default: return new TilePresenter(tile, view);
        }
    }
}