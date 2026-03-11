using System.Collections.Generic;

public interface IConnectionPresenter
{
    Stack<ConnectionResult> ConnectionHistory { get; }
    ConnectionSession Session { get; }
    void Initialize(IBoardPresenter board);
}