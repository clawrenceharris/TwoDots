using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// <summary>
/// Represents an active connection (or player "drag") between dots on the board.
/// Handles the state, interactions, and events that occur during the connection session,
/// including adding/removing dots to the path, managing color, tracking when a square (loop) is formed,
/// raising events for UI/logic updates, and exposing the current path and state for use by
/// game mechanics and rules.
/// </summary>

public sealed class Connection
{

    /// <summary>True if the connection is closed by revisiting an earlier dot.</summary>
    public bool IsSquare => Square != null;
    /// <summary>The current color of the connection.</summary>
    public DotColor Color { get; private set; }

    /// <summary> The square that is created when the connection is closed by revisiting an earlier dot. </summary>
    public Square Square { get; private set; }
    /// <summary>True when a session is active (between Begin and End/Cancel).</summary>
    public bool IsActive { get; private set; }
    
  

    private readonly List<string> _path;
    public string CurrentDot => _path.Last();

    /// <summary>Ordered, unique dot IDs in the path.</summary>
    public List<string> Path => _path;

    /// <summary>The dot IDs to hit from the resulting connection.</summary>
    public List<string> DotsToHit { get; private set; }

    /// <summary>Raised when the connection is started and before the first dot is added to the path.</summary>
    public event Action OnConnectionStarted;
    
    /// <summary>Raised when the path changes.</summary>
    public event Action OnPathChanged;

      /// <summary>Raised when a dot is removed from the path.</summary>
    public event Action<string> OnDotRemovedFromPath;
    
    /// <summary>Raised when a square is activated.</summary>
    public event Action<IReadOnlyList<string>> OnSquareActivated;
    /// <summary>Raised when a square is deactivated.</summary>
    public event Action<IReadOnlyList<string>> OnSquareDeactivated;

    /// <summary>Raised when the connection is completed.</summary>
    public event Action<ConnectionResult> OnConnectionCompleted;
     /// <summary>Raised when the color changes.</summary>
    public event Action<DotColor> OnColorChanged;

    /// <summary>Raised when a dot is added to the path.</summary>
    public event Action<string> OnDotAddedToPath;

    /// <summary>Raised when the connection is cancelled.</summary>
    public event Action OnConnectionCancelled;


    public Connection()
    {
        _path = new List<string>();
        Color = DotColor.Blank;
        Square = null;
        IsActive = false;
    }


    /// <summary>
    /// Start a new session with the given dot as the first node.   
    /// </summary>
    /// <param name="dot">The starting dot</param>
    public void StartSession(IDotPresenter dot)
    {
        CancelSession();
        IsActive = true;

        Append(dot);
        OnConnectionStarted?.Invoke();



    }
   
    /// <summary>
    /// Append a dot to the path.
    /// </summary>
    /// <param name="dot">The dot to append</param>
    public void Append(IDotPresenter dot)
    {
        _path.Add(dot.Dot.ID);

        OnDotAddedToPath?.Invoke(dot.Dot.ID);
        OnPathChanged?.Invoke();

    }

    /// <summary>
    /// Backtrack the connection by removing the last dot from the path.
    /// </summary>
    public void Backtrack()
    {
        if (_path.Count == 0) return;
        string dotToRemove = _path[^1];
        _path.RemoveAt(_path.Count - 1);
        if (!_path.Contains(dotToRemove)) OnDotRemovedFromPath?.Invoke(dotToRemove);
        OnPathChanged?.Invoke();
        
    }

    
    /// <summary>
    /// Deactivate the square.
    /// </summary>
    public void DeactivateSquare()
    {
        // All the dots that would have been hit from the square excluding the dots that are still in the path
        List<string> dotsToDeactivate = new(Square.DotsToHitBySquare.ToList());
        OnSquareDeactivated?.Invoke(dotsToDeactivate);
        
        Square.Deactivate();
        Square = null;
       
    }
    /// <summary>
    /// Activate the square.
    /// </summary>
    /// <param name="square">The square activate</param>
    public void ActivateSquare(Square square)
    {
        Square = square;
        Square.Activate();
        OnSquareActivated?.Invoke(Square.AllDotsToHit);
    }

    /// <summary>
    /// Gathers all entities that should be "hit" by the current connection, including reached dots
    /// and any additional targets that propagate via ITargetable neighbors (chain reactions).
    ///     - If path is empty or BoardService unavailable, returns an empty list.
    ///     - Adds any hit-worthy entity (via Hittable.ShouldHit()) in the path, square, or as discovered targets.
    /// </summary>
    /// <returns>
    ///     A <see cref="List{String}"/> of all entity IDs to be hit as part of the current connection.
    /// </returns>
    public void EndSession()
    {
        Square?.Commit();

        OnConnectionCompleted?.Invoke(new ConnectionResult(this));

    }


    /// <summary>
    /// Cancel the session. Resets path, color, 
    /// and square and becomes inactive
    /// </summary>

    public void CancelSession()
    {
        IsActive = false;
        _path.Clear();
        Color = DotColor.Blank;
        Square = null;
        OnConnectionCancelled?.Invoke();
    }


    
    /// <summary>
    /// Get the color of the connection.
    /// </summary>
    /// <returns>
    /// The color of the connection.
    /// </returns>
    private DotColor GetConnectionColor()
    {
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return DotColor.Blank;
        var board = boardService.BoardPresenter;
        // find the color of the connection
        foreach (var dotId in _path)
        {
            var dot = board.GetDot(dotId);

            // skip if not a color dot
            if (!dot.Dot.DotType.IsColorable()) continue;
            if (dot.Dot.TryGetModel(out Colorable colorable))
            {
                // skip if the dot's color is blank. We only care about dots with a definitive color
                if (colorable.Color.IsBlank()) continue;

                // set color to the first colorable dot found
                return colorable.Color;
            }
        }
        // if no color dot found, return blank color
        return DotColor.Blank;
    }
    
    /// <summary>
    /// Update the color of the connection.
    /// </summary>
    public void UpdateColor()
    {
        var newColor = GetConnectionColor();
        if (newColor != Color)
        {
            Color = newColor;
            OnColorChanged?.Invoke(Color);
        }
    }

}