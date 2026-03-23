using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A factory for creating tiles and their presenters
/// </summary>
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
                    
                    var hittable = tile.AddModel(new Hittable(tile, new AdjacentToConnectionRule()));
                    tile.AddModel(new Clearable(tile , hittable));
                    return tile;
            }
            case TileType.OneSidedBlock:
                {
                    var tile = new Tile(type, new Vector2Int(data.Col, data.Row));
                    var direction = data.GetProperty<int[]>(DotsObject.Property.Directions);
                    var hittable = tile.AddModel(new Hittable(tile, new FacingAdjacentConnectionRule()));
                    tile.AddModel(new Clearable(tile, hittable));
                    tile.AddModel(new Directional(tile, new Vector2Int(direction[0], direction[1])));
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
            case TileType.OneSidedBlock:
                {
                    var presenter = new OneSidedBlockPresenter(tile, view);
                    presenter.AddPresenter(new DirectionalPresenter(tile, view));
                    presenter.AddPresenter(new HittablePresenter(tile, view));
                    presenter.AddPresenter(new ClearablePresenter(tile, view));
                    return presenter;
                }
            default: return new TilePresenter(tile, view);
        }
    }
}