using System;
using System.Collections.Generic;
using UnityEngine;

public class DotView : EntityView
{
    
    private Dot _dot;

    public virtual void Init(Dot dot)
    {
        _dot = dot;
        name = $"{_dot.DotType} Dot ({_dot.GridPosition.x}, {_dot.GridPosition.y})";
    }

}