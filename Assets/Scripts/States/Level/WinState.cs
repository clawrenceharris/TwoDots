
using System;
using UnityEngine;


/// <summary>
/// Represents the active gameplay state where the player and enemies can move and interact.
/// </summary>
/// <remarks>
/// In this state, player and enemy movement is enabled, and collisions between the player and other objects are checked.
/// </remarks>
public class WinState : State<LevelStateManager>
{
    public WinState(LevelStateManager context) : base(context)
    {
    }

    public override void EnterState()
    {
        InputRouter.Gate.SetEnabled(false);
        // TODO: Save player data for this level 
    }



    public override void UpdateState()
    {
    }
    public override void ExitState()
    {

    }

   
    
}