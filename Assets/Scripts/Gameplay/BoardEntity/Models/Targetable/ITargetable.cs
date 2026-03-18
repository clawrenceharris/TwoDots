using System.Collections.Generic;

public interface ITargetable
{
    List<IBoardEntity> GetTargets(IBoardPresenter board, Connection connection);
}