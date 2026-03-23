using System.Collections.Generic;

public interface IConnectionPresenter
{
    /// <summary>
    /// The connection instance.
    /// </summary>
    Connection Connection { get; }
    void Initialize(IBoardPresenter board);
}