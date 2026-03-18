using System.Linq;
using UnityEngine;

public class TileSpriteController : SpriteController
{
    [SerializeField] private TileSpriteSet _tileSpriteSet;

    /// <summary>
    /// Updates the tile sprite based on neighboring tiles.
    /// </summary>
    /// <param name="boardModel">The board model to query for neighbors</param>
    /// <param name="gridPosition">The grid position of this tile</param>
    public void UpdateSprite(IBoardPresenter board, Vector2Int gridPosition)
    {
        if (_renderer == null)
        {
            Debug.LogWarning("[TileSpriteController] SpriteRenderer is null, cannot update sprite");
            return;
        }

        if (_tileSpriteSet == null)
        {
            return;
        }

        int spriteIndex = TileSpriteSelector.GetSpriteIndex(board, gridPosition);
        Sprite sprite = _tileSpriteSet.GetSprite(spriteIndex);

        if (sprite == null)
        {
            Debug.LogWarning($"[TileSpriteController] Sprite at index {spriteIndex} is null in TileSpriteSet {_tileSpriteSet.name}");
            return;
        }

        _renderer.BaseRenderer.sprite = sprite;
    }
}