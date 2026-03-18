using System;
using UnityEngine;
/// <summary>
/// A view component for a tile entity on the board
/// </summary>
public class TileView : EntityView
{   
    /// <summary>The sprite controller for the tile</summary>
    protected TileSpriteController _spriteController;
    /// <summary>The tile entity</summary>
    protected Tile _tile;

   
    /// <summary>
    /// Initializes the tile view
    /// </summary>
    /// <param name="tile">The tile entity</param>
    public virtual void Init(Tile tile)
    {
        _tile = tile;
        name = $"{_tile.TileType} Tile ({_tile.GridPosition.x}, {_tile.GridPosition.y})";
        TryGetComponent(out _spriteController);


    }

    public void UpdateTileSprite(IBoardPresenter board)
    {
        if(_spriteController == null) return;
        _spriteController.UpdateSprite(board, _tile.GridPosition);
    }
}