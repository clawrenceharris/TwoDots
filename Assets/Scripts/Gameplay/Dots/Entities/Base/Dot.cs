using System;
using UnityEngine;

public class Dot : BoardEntity
{
    
    private readonly DotType _dotType;
    public DotType DotType => _dotType;

    public Dot(DotType type, Vector2Int position) : base(position)
    {
        _dotType = type;
    }
}