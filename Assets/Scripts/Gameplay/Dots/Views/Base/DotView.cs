using System;
using System.Collections.Generic;
using UnityEngine;

public class DotView : MonoBehaviour
{
    
    private Dot _dot;
    private DotsRenderer _renderer;
    public DotsRenderer Renderer => _renderer;

    public virtual void Init(Dot dot)
    {
        _dot = dot;
        TryGetComponent(out _renderer);
        name = $"{_dot.DotType} Dot ({_dot.GridPosition.x}, {_dot.GridPosition.y})";
    }

}