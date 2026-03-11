using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// A step in a connection session.
/// </summary>
public record ConnectionStep
{
    public readonly IDotPresenter Dot;
    public List<string> DotsToHit;

    public ConnectionStep(IDotPresenter dot, ConnectionSession session)
    {
        Dot = dot;
        DotsToHit = new List<string>();
        CollectDotsToHitFromDot(session);

    }
    public void CollectDotsToHitFromSquare(ConnectionSession session)
    {
        if(session.Square == null) return;
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return;
        var board = boardService.BoardPresenter;
        // add all dots to hit based on square hit conditions
        foreach (var dotId in session.Square.DotIdsToHit)
        {
            var dot = board.GetDot(dotId);
            if (dot == null) continue;
            if(DotsToHit.Contains(dot.Dot.ID)) continue;
            
            var neighbors = board.GetDotNeighbors(dot.Dot.GridPosition, includesDiagonals: false);
            foreach (var neighbor in neighbors)
            {
                if (neighbor == null) continue;
                if (neighbor.Dot.TryGetModel(out Hittable hittable) &&

               hittable.Conditions.Contains(HitConditionType.AdjacentToSquare) || hittable.Conditions.Contains(HitConditionType.AdjacentToConnection))
                {
                    DotsToHit.Add(neighbor.Dot.ID);
                }
            }

            Debug.Log($"Dots to hit from square: {DotsToHit.Count}");

        }
    }
    /// <summary>
    /// Collects the dots to hit from the target dot we just connected to.
    /// </summary>
    /// <param name="session">The connection session</param>
    public void CollectDotsToHitFromDot(ConnectionSession session)
    {
        if (!ServiceProvider.Instance.TryGetService<BoardService>(out var boardService)) return;
        var board = boardService.BoardPresenter;
        CollectDotsToHitFromSquare(session);

        // add dot if it should be hit by being in the connection
        if (Dot.Dot.TryGetModel(out Hittable hittable) && hittable.Conditions.Contains(HitConditionType.Connection))
        {
            Debug.Log($"Dot should be hit by connection: {Dot.Dot.ID}");
            DotsToHit.Add(Dot.Dot.ID);
        }

        // add all dots to hit based on connection adjacency
        var neighbors = board.GetNeighbors(Dot.Dot.GridPosition, includesDiagonals: false);
        foreach (var neighbor in neighbors)
        {
            if (neighbor == null) continue;
            if (DotsToHit.Contains(neighbor.ID)) continue;
            if (neighbor.TryGetModel(out Hittable hittableNeighbor) && hittableNeighbor.Conditions.Contains(HitConditionType.AdjacentToConnection))
            {
                Debug.Log($"Neighbor should be hit by connection: {neighbor.GridPosition}");
                DotsToHit.Add(neighbor.ID);
            }
        }
        Debug.Log($"Dots to hit from dot: {DotsToHit.Count}");

    }
}

/// <summary>
/// The connection session.
/// </summary>
public class ConnectionSession
{
    /// <summary>The path of the current connection.</summary>
    public readonly Stack<ConnectionStep> Steps;
    /// <summary>True if the connection is closed by revisiting an earlier dot.</summary>
    public bool IsSquare => Square != null;
    /// <summary>The current color of the connection.</summary>
    public DotColor Color { get; private set; }

    /// <summary> The square that is created when the connection is closed by revisiting an earlier dot. </summary>
    public Square Square { get; private set; }
    /// <summary>The current step in the session.</summary>
    public ConnectionStep CurrentStep => Steps.Peek();
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

    public List<string> Path => _path;

    public event Action OnPathChanged;

    public ConnectionSession()
    {
        Steps = new Stack<ConnectionStep>();
        _path = new List<string>();
        Color = DotColor.Blank;
        Square = null;
    }
     public void BeginSession(IDotPresenter dot)
    {
        Steps.Clear();
        _path.Clear();
        Color = DotColor.Blank;
        Square = null;
        IsActive = true;
        Steps.Push(new ConnectionStep(dot, this));
        Path.Add(dot.Dot.ID);
        OnPathChanged?.Invoke();
    }
    public void Append(IDotPresenter dot)
    {
        Path.Add(dot.Dot.ID);
        Steps.Push(new ConnectionStep(dot, this));

        OnDotAddedToPath?.Invoke(dot.Dot.ID);
        OnPathChanged?.Invoke();

    }

    public void Backtrack()
    {
        if (Path.Count == 0) return;
        Steps.Pop();
        string dotToRemove = Path[^1];
        Path.RemoveAt(Path.Count - 1);

        if (!Path.Contains(dotToRemove)) OnDotRemovedFromPath?.Invoke(dotToRemove);
        OnPathChanged?.Invoke();
        
    }

    
    
    public void DeactivateSquare()
    {



        // All the dots that would have been hit from the square excluding the dots that are still in the path
        List<string> dotsToDeactivate = new(Square.DotIdsToHit.Where(id => !Path.Contains(id)));
        OnSquareDeactivated?.Invoke(dotsToDeactivate);
        
        Square.Deactivate();
        Square = null;
       
    }
    public void ActivateSquare(Square square)
    {
        Square = square;
        Square.Activate();

        OnSquareActivated?.Invoke(Square.DotIdsToHit);
    }
    public void EndSession()
    {
        IsActive = false;
        Square?.Commit();
        OnConnectionCompleted?.Invoke(new ConnectionResult(this));
    }
    public List<string> GetAllDotsToHit()
    {
        var dotsToHit = Steps.ToList().SelectMany(step => step.DotsToHit).Distinct().ToList();
        Debug.Log($"Dots to hit: {dotsToHit.Count}");
        if (IsSquare)
        {
            dotsToHit.AddRange(Square.DotIdsToHit);
        }
       
        return dotsToHit.Distinct().ToList();
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