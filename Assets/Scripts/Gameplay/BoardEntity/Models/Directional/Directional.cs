using System;
using UnityEngine;

public class Directional : ModelBase, IDirectional
{
    public Directional(Dot dot) : base(dot)
    {
    }

    public int DirectionX { get; private set; }
    public int DirectionY { get; private set; }
   
    public void SetDirection(int dirX, int dirY)
    {
        DirectionX = dirX;
        DirectionY = dirY;
    }

    public Vector2Int FindBestDirection(IBoardPresenter board, Func<Dot, bool> isValidTarget)
    {
        throw new NotImplementedException();
    }

    public Vector3 ToRotation(int dirX, int dirY)
    {
        throw new NotImplementedException();
    }
}