using System;
using UnityEngine;

/// <summary>
/// A dot entity on the board
/// </summary>
public class Dot : BoardEntity
{
    /// <summary>The type of dot</summary>
    private readonly DotType _dotType;
    /// <summary> The type of dot  </summary>
    public DotType DotType => _dotType;
    /// <summary>
    /// Initializes a new instance of the <see cref="Dot"/> class.
    /// </summary>
    /// <param name="type">The type of dot</param>
    /// <param name="position">The position of the dot on the board</param>
    public Dot(DotType type, Vector2Int position) : base(position)
    {
        _dotType = type;
    }
    public override T GetEntityType<T>()
    {
        return (T)(object)DotType;
    }
   
}