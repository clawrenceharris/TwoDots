using System;
using UnityEngine;

/// <summary>
/// A model associated with a board entity that can face or move in a specific direction.
/// </summary>
public class Directional : ModelBase, IDirectional
{
    public Vector2Int FacingDirection { get; private set; }
    public Directional(BoardEntity entity, Vector2Int direction) : base(entity)
    {
        FacingDirection = direction;
    }    
    public Vector2Int FindBestDirection(IBoardPresenter board, Func<Dot, bool> isValidTarget)
    {
        Vector2Int right = new(-FacingDirection.y, FacingDirection.x);
        Vector2Int left = new(FacingDirection.y, -FacingDirection.x);

        //get the dot that is 90 degrees to the left of the dot (y, -x)
        IDotPresenter targetDot = board.GetDotAt(left.x + _entity.GridPosition.x, left.y + _entity.GridPosition.y);

        if (isValidTarget(targetDot.Dot))
        {
            return left;
        }
        else
        {
            return right;

        }
    }

    public Vector3 ToRotation(int dirX, int dirY)
    {
        return new Vector3(0, 0, Mathf.Atan2(dirY, dirX) * Mathf.Rad2Deg);
    }
    public void SetDirection(int dirX, int dirY)
    {
        FacingDirection = new Vector2Int(Mathf.Clamp(dirX, -1, 1), Mathf.Clamp(dirY, -1, 1));
    }

}