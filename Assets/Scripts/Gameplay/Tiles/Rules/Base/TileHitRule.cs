using System.Collections.Generic;

/// <summary>
/// A rule that determines if a hittable tile should be hit.
/// </summary>
public class TileHitRule : IHitRule
{
   
    public virtual bool CanHit(IBoardPresenter board, Connection connection, string tileId)
    {
        return CheckBaseRules(new List<IHitRule> { new AdjacentToConnectionRule() }, board, connection, tileId);
    }
    
    public virtual bool CheckBaseRules(List<IHitRule> rules, IBoardPresenter board, Connection connectionSession, string tileId)
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