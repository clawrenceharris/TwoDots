using UnityEngine;

public class OneSidedBlockHitRule : TileHitRule
{
    public override bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string tileId)
    {
        var entity = board.GetEntity(tileId);
        if (entity == null) return false;
        if (entity.GetEntity().TryGetModel(out Directional directional))
        {
            var targetPosition = new Vector2Int(directional.DirectionX + entity.GetEntity().GridPosition.x, directional.DirectionY + entity.GetEntity().GridPosition.y);
            var targetDot = board.GetDotAt(targetPosition);
            if (targetDot == null) return false;
            
            return false;
        }
        return false;
    }
    
}