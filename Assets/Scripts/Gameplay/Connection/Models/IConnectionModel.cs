using System;
using System.Collections.Generic;

public interface IConnectionModel
{
    /// <summary>Current ordered path of dots in this session (empty if no active session).</summary>
    IReadOnlyList<string> Path { get; }
    /// <summary> Set of unique dot IDs in the path. </summary>
    IReadOnlyList<string> DotIdsInPath { get; }
    
    /// <summary>The connection instance.</summary>
    Connection Connection { get; }


    /// <summary>Try to extend, backtrack, or close cycle with the given dot. Returns true if path changed.</summary>
    bool TryAppend(IDotPresenter dot);

    /// <summary>End the session (e.g. on pointer up).</summary>
    void End();

    /// <summary>Cancel the session without emitting completion (e.g. cleanup).</summary>
    void Cancel();

    /// <summary>Update the color of the connection.</summary>
    void UpdateColor();
    
    /// <summary>Try to backtrack the connection.</summary>
    bool TryBacktrack(IDotPresenter dot);
    void Initialize(IBoardPresenter board);

    /// <summary>Try to start a session with the given dot as the first node.</summary>
    bool TryBegin(IDotPresenter dot);
}