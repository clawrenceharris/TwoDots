using System.Collections.Generic;
/// <summary>
/// A model associated with a board entity that can target or trigger other dots to be hit.
/// </summary>
public class Targetable : ModelBase, ITargetable
{
    public Targetable(BoardEntity entity) : base(entity)
    {
    }

    public virtual List<IBoardEntity> GetTargets(IBoardPresenter board, Connection connection)
    {
        return new List<IBoardEntity>();
    }
}