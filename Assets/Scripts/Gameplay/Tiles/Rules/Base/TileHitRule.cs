using System.Collections.Generic;

public abstract class TileHitRule : IHitRule
{
    public virtual bool CanHit(IBoardPresenter board, ConnectionSession connectionSession, string tileId)
    {
        return CheckBaseRules(new List<IHitRule> { new AdjacentToSquareRule(), new AdjacentToConnectionRule() }, board, connectionSession, tileId);
    }
    
    public virtual bool CheckBaseRules(List<IHitRule> rules, IBoardPresenter board, ConnectionSession connectionSession, string tileId)
    {
        foreach (var rule in rules)
        {
            if (rule.CanHit(board, connectionSession, tileId))
            {
                return true;
            }
        }
        return false;
    }
}