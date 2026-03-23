using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A view component for a dot entity on the board
/// </summary>
public class DotView : EntityView
{
    private Dot _dot;

    /// <summary>
    /// Initializes the dot view
    /// </summary>
    /// <param name="dot">The dot entity</param>
    public virtual void Init(Dot dot)
    {
        _dot = dot;
        name = $"{_dot.DotType} Dot ({_dot.GridPosition.x}, {_dot.GridPosition.y})";
    }

}