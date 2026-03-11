using System;
using UnityEngine;

public interface IDirectional
{
    int DirectionX { get; }
    int DirectionY { get; }
     /// <summary>
    /// Determines and returns the optimal direction for the directional object to face, 
    /// based on the validity of neighboring game objects in each direction.
    /// </summary>
    /// <param name="board">The current game board.</param>
    /// <param name="isValidTarget">A function that evaluates the validity of a neighboring game object as a target.</param>
    /// <returns>The optimal direction vector.</returns>
    Vector2Int FindBestDirection(IBoardPresenter board, Func<Dot, bool> isValidTarget);
    Vector3 ToRotation(int dirX, int dirY);
    void SetDirection(int dirX, int dirY);
}