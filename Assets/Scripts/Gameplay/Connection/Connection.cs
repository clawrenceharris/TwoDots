using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// A step in the active connection session that holds the current (last) dot in the connection path.
/// </summary>
public record ConnectionStep
{
    public readonly IDotPresenter Dot;
    public List<string> ToHit;
    public List<string> ToPreview;
    public List<string> ToClear;
    private readonly Connection _session;
    public ConnectionStep(IDotPresenter dot, Connection session)
    {
        Dot = dot;
        _session = session;

    }
   
}

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
     /// <summary>Raised when the color changes.</summary>
    public event Action<DotColor> OnColorChanged;

    /// <summary>Raised when a dot is added to the path.</summary>
    public event Action<string> OnDotAddedToPath;

    /// <summary>Raised when a dot is removed from the path.</summary>
    public event Action<string> OnDotRemovedFromPath;
    
    /// <summary>Raised when a square is activated.</summary>
    public event Action<IReadOnlyList<string>> OnSquareActivated;
    /// <summary>Raised when a square is deactivated.</summary>
    public event Action<IReadOnlyList<string>> OnSquareDeactivated;

    /// <summary>Raised when the connection is completed.</summary>
    public event Action<ConnectionResult> OnConnectionCompleted;

    private readonly List<string> _path;
    public string CurrentDot => Path.Last();

    /// <summary>Ordered, unique dot IDs in the path.</summary>
    public List<string> Path => _path;

    /// <summary>The dot IDs to hit from the resulting connection.</summary>
    public List<string> DotsToHit { get; private set; }

    /// <summary>Raised when the connection is started and before the first dot is added to the path.</summary>
    public event Action OnConnectionStarted;
    
    /// <summary>Raised when the path changes.</summary>
    public event Action OnPathChanged;

    public Connection()
    {
        _path = new List<string>();
        Color = DotColor.Blank;
        Square = null;
       
    }
    public void BeginSession(IDotPresenter dot)
    {
        _path.Clear();
        Color = DotColor.Blank;
        Square = null;
        IsActive = true;
        OnConnectionStarted?.Invoke();
        Append(dot);

    }
    public void Append(IDotPresenter dot)
    {
        Path.Add(dot.Dot.ID);

        OnDotAddedToPath?.Invoke(dot.Dot.ID);
        OnPathChanged?.Invoke();

    }

    public void Backtrack()
    {
        if (Path.Count == 0) return;
        string dotToRemove = Path[^1];
        Path.RemoveAt(Path.Count - 1);
        if (!Path.Contains(dotToRemove)) OnDotRemovedFromPath?.Invoke(dotToRemove);
        OnPathChanged?.Invoke();
        
    }

    
    
    public void DeactivateSquare()
    {
        // All the dots that would have been hit from the square excluding the dots that are still in the path
        List<string> dotsToDeactivate = new(Square.DotsToHit.Where(id => !Path.Contains(id)));
        OnSquareDeactivated?.Invoke(dotsToDeactivate);
        
        Square.Deactivate();
        Square = null;
       
    }
    public void ActivateSquare(Square square)
    {
        Square = square;
        Square.Activate();
        OnSquareActivated?.Invoke(Square.DotsToHit);
    }
    /// <summary>
    ///     Gathers all entities that should be "hit" by the current connection, including reached dots
    ///     and any additional targets that propagate via ITargetable neighbors (chain reactions).
    ///     - If path is empty or BoardService unavailable, returns an empty list.
    ///     - Adds any hit-worthy entity (via Hittable.ShouldHit()) in the path, square, or as discovered targets.
    /// </summary>
    /// <returns>
    ///     A <see cref="List{String}"/> of all entity IDs to be hit as part of the current connection.
    /// </returns>

    private List<string> GetAllToHit()
    {
        if(Path.Count <= 1) return new List<string>();
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return new List<string>();
        var toHit = new List<string>();
        var board = boardService.BoardPresenter;
        var queue = new Queue<string>();
        var visited = new HashSet<string>();
        var dotsInConnection = Path.Concat(Square?.DotsToHit ?? new List<string>()).Distinct().ToList();
        foreach (var dotId in dotsInConnection)
        {
            queue.Enqueue(dotId);
            while (queue.Count > 0)
            {
                var entityId = queue.Dequeue();
                var entity = board.GetEntity(entityId);
                if (entity == null) continue;
                if (visited.Contains(entityId)) continue;
                if (entity.Entity.TryGetModel(out Hittable hittable) && hittable.ShouldHit())
                {
                    toHit.Add(entityId);
                }
                if (entity.Entity.TryGetModel(out Targetable targetable))
                {

                    var targets = targetable.GetTargets(board, this);
                    foreach (var target in targets)
                    {
                        var targetEntity = board.GetEntity(target.ID);
                        if (targetEntity.Entity.TryGetModel(out Hittable hittableTarget) && hittableTarget.ShouldHit())
                        {
                            queue.Enqueue(target.ID);
                        }
                    }
                }
                visited.Add(entityId);
            }
        }
        return toHit.ToList();
    }
    public void EndSession()
    {
        IsActive = false;
        Square?.Commit();
        DotsToHit = GetAllToHit();
        OnConnectionCompleted?.Invoke(new ConnectionResult(this));
    }
    
    private DotColor GetConnectionColor()
    {
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return DotColor.Blank;
        var board = boardService.BoardPresenter;
        // find the color of the connection
        foreach (var dotId in Path)
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