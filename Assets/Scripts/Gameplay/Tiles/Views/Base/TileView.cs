using System;
using UnityEngine;

public class TileView : MonoBehaviour
{
    protected TileSpriteController _spriteController;
    protected Tile _tile;

    private DotsRenderer _renderer;
    public DotsRenderer Renderer => _renderer;


    public virtual void Init(Tile tile)
    {
        _tile = tile;
        name = $"{_tile.TileType} Tile ({_tile.GridPosition.x}, {_tile.GridPosition.y})";
        TryGetComponent(out _spriteController);
        TryGetComponent(out _renderer);

        transform.localScale = Vector3.one * BoardView.TileSize;

    }

    public void UpdateTileSprite(IBoardPresenter board)
    {
        Debug.Log($"[TileView] Updating tile sprite for {_tile.ID}");
        _spriteController.UpdateSprite(board, _tile.GridPosition);
    }
}