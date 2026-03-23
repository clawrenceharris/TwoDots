using System.Collections.Generic;
using System.Linq;

/// <summary>
/// A model associated with a normal dot that can target or trigger other dots to be hit.
/// </summary>
public class TargetableNormalDot : Targetable
{
    public TargetableNormalDot(BoardEntity entity) : base(entity)
    {
    }
    public override List<IBoardEntity> GetTargets(IBoardPresenter board, Connection connection)
    {
        var dotsInConnection = connection.Path.Concat(connection.Square?.AllDotsToHit ?? new List<string>()).Distinct().ToList();
        if (dotsInConnection.Contains(_entity.ID))
        {
            return board.GetNeighbors(_entity.GridPosition, includesDiagonals: false);
        }
        return new List<IBoardEntity>();
    }
}