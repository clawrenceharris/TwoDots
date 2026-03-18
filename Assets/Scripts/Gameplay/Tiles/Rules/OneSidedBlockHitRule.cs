using System.Linq;
using UnityEngine;

/// <summary>
/// A rule that determines if a one-sided block tile should be hit.
/// </summary>
public class OneSidedBlockHitRule : TileHitRule
{
    public override bool CanHit(IBoardPresenter board, Connection connection, string tileId)
    {
        var tile = board.GetTile(tileId);
        var neighbors = board.GetDotNeighbors(tile.Tile.GridPosition, includesDiagonals: false);
        var dotsInConnection = connection.Path;
        if(connection.IsSquare)
        {
            dotsInConnection = dotsInConnection.Concat(connection.Square.DotsToHit).Distinct().ToList();
        }
        Directional directional = tile.Tile.GetModel<Directional>();
        foreach (var dot in neighbors)
        {
            if (!dotsInConnection.Contains(dot.Dot.ID))
            {
                continue;
            }
            
            if (!dot.Dot.TryGetModel(out Hittable hittableDot))
            {
                continue;
            }

            if (directional.FacingDirection + tile.Tile.GridPosition != dot.Dot.GridPosition)
            {
                continue;
            }
            if (hittableDot.ShouldClearAfterHit()) return true;
        }
        return false;
    }
    
}