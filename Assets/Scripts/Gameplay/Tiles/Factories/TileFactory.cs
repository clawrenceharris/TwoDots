using System;
using UnityEngine;

public class TileFactory
{
    public static Tile CreateTile(DotsObject data)
    {
        var type = LevelLoader.FromJsonType<TileType>(data.Type);
        switch (type)
        {
            
            default:
                return new Tile(type, new Vector2Int(data.Col, data.Row));
        }
    }

    public static ITilePresenter CreateTilePresenter(Tile tile, TileView view)
    {
        switch (tile.TileType)
        {
            default: return new TilePresenter(tile, view);
        }
    }
}