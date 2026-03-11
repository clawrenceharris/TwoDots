using System;
using System.Collections.Generic;
using UnityEngine;

public class TileFactory
{
    public static Tile CreateTile(DotsObject data)
    {
        var type = LevelLoader.FromJsonType<TileType>(data.Type);
        switch (type)
        {
            case TileType.Block:
                {
                    var tile = new Tile(type, new Vector2Int(data.Col, data.Row));
                    
                    tile.AddModel(new Hittable(tile, new Clearable(tile), hitMax: 1, conditions: new List<HitConditionType> { HitConditionType.AdjacentToConnection, HitConditionType.AdjacentToSquare }));
                    return tile;
            }
            default:
                return new Tile(type, new Vector2Int(data.Col, data.Row));
        }
    }

    public static TilePresenter CreateTilePresenter(Tile tile, TileView view, IBoardPresenter board)
    {
        switch (tile.TileType)
        {
            case TileType.Block:{
                var presenter = new TilePresenter(tile, view);
                presenter.AddPresenter(new HittablePresenter(tile, view));
                presenter.AddPresenter(new ClearablePresenter(tile, view));
                return presenter;
            }
            default: return new TilePresenter(tile, view);
        }
    }
}