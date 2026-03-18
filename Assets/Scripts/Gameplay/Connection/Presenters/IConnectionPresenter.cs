using System.Collections.Generic;

public interface IConnectionPresenter
{
    Stack<ConnectionResult> ConnectionHistory { get; }
    Connection Connection { get; }
    void Initialize(IBoardPresenter board);
}