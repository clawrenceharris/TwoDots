using System;
using System.Collections.Generic;

public interface IConnectionModel
{
    /// <summary>
    /// The dot IDs to hit in by the connection from the square.
    /// </summary>
    IReadOnlyList<string> DotsToHitFromSquare { get; }
    /// <summary>Current ordered path of dots in this session (empty if no active session).</summary>
    IReadOnlyList<IDotPresenter> Path { get; }

    /// <summary>True when a session is active (between Begin and End/Cancel).</summary>
    bool IsSessionActive { get; }

    /// <summary>Raised when the path changes (segment added or removed).</summary>
    event Action OnPathChanged;

    /// <summary>Raised when the session ends with pointer up; payload describes the completed path.</summary>
    event Action<ConnectionContext> OnConnectionCompleted;

    /// <summary>Raised when the color changes.</summary>
    event Action<DotColor> OnColorChanged;

    /// <summary>Raised when a dot is added to the path.</summary>
    event Action<string> OnDotAddedToPath;

    /// <summary>Raised when a dot is removed from the path.</summary>
    event Action<string> OnDotRemovedFromPath;
    event Action<IReadOnlyList<string>> OnSquareActivated;
    event Action<IReadOnlyList<string>> OnSquareDeactivated;
    /// <summary>Current color of the connection.</summary>
    DotColor CurrentColor { get; }

    /// <summary>True if the connection is closed by revisiting an earlier dot.</summary>
    bool IsSquare { get; }
    
    /// <summary>Start a session with the given dot as the first node.</summary>
    void Begin(IDotPresenter dot);

    /// <summary>Try to extend, backtrack, or close cycle with the given dot. Returns true if path changed.</summary>
    bool TryAppend(IDotPresenter dot);

    /// <summary>End the session and emit completion payload (e.g. on pointer up).</summary>
    void End();

    /// <summary>Cancel the session without emitting completion (e.g. cleanup).</summary>
    void Cancel();

    /// <summary>Update the color of the connection.</summary>
    void UpdateColor();
    
    /// <summary>Try to backtrack the connection.</summary>
    bool TryBacktrack(IDotPresenter dot);
}