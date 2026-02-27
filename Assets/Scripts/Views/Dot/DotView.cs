using System;
using System.Collections.Generic;
using UnityEngine;

public class DotView : MonoBehaviour
{
    private Dot _dot;
    private DotRenderer _dotRenderer;
    public DotRenderer DotRenderer
    {
        get
        {
            if (_dotRenderer == null)
                _dotRenderer = TryGetComponent(out DotRenderer renderer) ? renderer : null;
            return _dotRenderer;
        }
    }

    public virtual void Init(Dot dot)
    {
        _dot = dot;
        name = $"{_dot.DotType} Dot ({_dot.GridPosition.x}, {_dot.GridPosition.y})";
    }

}