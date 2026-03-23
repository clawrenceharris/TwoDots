using System.Collections.Generic;

public abstract class Rule : IRule
{
    protected readonly List<IRule> Rules = new();
    public virtual bool CanHit(IBoardPresenter board, Connection connection, string dotId)
    {
        foreach (var rule in Rules)
        {
            if (rule.CanHit(board, connection, dotId))
            {
                return true;
            }
        }
        return false;
    }
}