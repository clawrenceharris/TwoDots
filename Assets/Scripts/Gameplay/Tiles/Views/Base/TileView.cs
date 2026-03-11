using System;
using UnityEngine;

public class TileView : EntityView
{
    protected TileSpriteController _spriteController;
    protected Tile _tile;



    public virtual void Init(Tile tile)
    {
        _tile = tile;
        name = $"{_tile.TileType} Tile ({_tile.GridPosition.x}, {_tile.GridPosition.y})";
        TryGetComponent(out _spriteController);

        transform.localScale = Vector3.one * BoardView.TileSize;

    }

    public void UpdateTileSprite(IBoardPresenter board)
    {
        _spriteController.UpdateSprite(board, _tile.GridPosition);
    }
}