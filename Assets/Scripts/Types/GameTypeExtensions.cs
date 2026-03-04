using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public static class GameTypeExtensions
{

    
    public static bool IsBlockable(this TileType type){
        return type == TileType.Block || type == TileType.OneSidedBlock;

    }

    public static bool IsLotusDot(this DotType type)
    {
        return type == DotType.Lotus;
    }

    public static bool IsWater(this TileType type)
    {
        return type == TileType.Water;
    }

    public static bool IsBlank(this DotType type)
    {
        return type == DotType.Blank || type == DotType.Clock || type == DotType.Magnet;
    }

    public static bool IsBlank(this DotColor color)
    {
        return color == DotColor.Blank;
    }
    public static bool IsConnectableDot(this DotType type)
    {
        return type == DotType.Normal || type.IsBlank() || type == DotType.Beetle;
    }
    

    public static bool ShouldBeHitBySquare(this DotType type)
    {
        
        return type == DotType.MoonStone;   
    }
    public static bool ShouldBeHitBySquare(this TileType type)
    {
        
        return false;   
    }

    
    public static bool IsBoardMechanicTile(this TileType type)
    {
        return type == TileType.Block || type == TileType.OneSidedBlock || type == TileType.EmptyTile;
    }
    
    public static bool IsColorable(this DotType type)
    {
        return type == DotType.Normal ||type == DotType.Lotus || type == DotType.Beetle || type == DotType.Blank;
    }

    public static bool IsNumerable(this DotType type)
    {
        return type == DotType.Clock;
    }
    public static bool IsNumerable(this TileType type)
    {
        return type == TileType.Zapper;
    }

    public static bool IsDirectional(this TileType type)
    {
        return type == TileType.OneSidedBlock;
    }

    public static bool IsDirectional(this DotType type)
    {
        return type == DotType.Beetle || type == DotType.Monster;
    }
    
    public static bool IsBomb(this DotType type)
    {
        return type == DotType.Bomb;
    }
    public static bool IsBasicDot(this DotType type)
    {
        return type == DotType.Blank || type == DotType.Normal;
    }
    public static bool IsClockDot(this DotType type)
    {
        return type == DotType.Clock;
    }

    public static bool IsMoonstoneDot(this DotType type)
    {
        return type == DotType.MoonStone;
    }

    
}


