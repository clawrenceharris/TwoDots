using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class OneSidedBlockPresenter : TilePresenter
{
    private Directional _directional;
    private List<string> _neighbors;

    /// <summary>
    /// Tracks how many neighboring dots are connected (not necessarily in the facing direction)
    /// </summary>
    private int _connectedNeighbors; 
    
    /// <summary>
    /// The number of connected neighbors exactly in the block's facing direction. (this will always either be 1 or 0)
    /// </summary>
    private int _facingConnectionCount;    // Was _targetCount; tracks neighbor connections specifically in the block's facing direction
   
    /// <summary>
    /// True if there is at least one connection in the block's facing direction.
    /// </summary>
    private bool HasFacingConnection => _facingConnectionCount > 0;
    

    
    public OneSidedBlockPresenter(Tile tile, TileView view) : base(tile, view)
    {
    }
    public override void Initialize(IBoardPresenter board)
    {
        base.Initialize(board);
        _directional = _entity.GetModel<Directional>();
        TileView.transform.rotation = Quaternion.Euler(_directional.ToRotation(_directional.FacingDirection.x, _directional.FacingDirection.y));
        if (ServiceProvider.Instance.TryGetService<ConnectionService>(out var connectionService))
        {
            connectionService.ActiveConnection.OnConnectionStarted += OnConnectionStarted;
            connectionService.ActiveConnection.OnDotAddedToPath += OnDotAddedToPath;
            connectionService.ActiveConnection.OnDotRemovedFromPath += OnDotRemovedFromPath;
            connectionService.ActiveConnection.OnConnectionCompleted += OnConnectionCompleted;
        }
    }

    private void OnConnectionStarted()
    {
        _neighbors = _board.GetDotNeighbors(_entity.GridPosition, false).Select(neighbor => neighbor.Dot.ID).ToList();
    }

    private void OnDotRemovedFromPath(string dotId)
    {

        if (!_neighbors.Contains(dotId)) return;
        _connectedNeighbors--;
        var neighbor = _board.GetDot(dotId);
        
        if (_directional.FacingDirection + _entity.GridPosition == neighbor.Dot.GridPosition)
        {
            _facingConnectionCount--;
        }
    }

    private void OnDotAddedToPath(string dotId)
    {
        if (!_neighbors.Contains(dotId)) return;
        var neighbor = _board.GetDot(dotId);
        _connectedNeighbors++;
        if (_directional.FacingDirection + _entity.GridPosition == neighbor.Dot.GridPosition)
        {
            _facingConnectionCount++;
        }
    }

    private void OnConnectionCompleted(ConnectionResult result)
    {
        if(result.DotIdsInPath.Count <= 1) return;
        if (_connectedNeighbors == 0) return;


        /// If there are candidates (connected neighbors) and no targets, shake the block.
        if (_connectedNeighbors > 0 && !HasFacingConnection)
        {
            // Shake with a single push in a random horizontal direction and return with elastic ease
            Vector3 originalPos = View.transform.localPosition;
            float pushDistance = 0.35f;
            int direction = UnityEngine.Random.value < 0.5f ? -1 : 1;
            Vector3 pushVector = new Vector3(direction * pushDistance, 0f, 0f);

            View.transform.DOLocalMove(originalPos + pushVector, 0.12f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    /// Return to original position with a bounce ease
                    View.transform.DOLocalMove(originalPos, 0.5f)
                        .SetEase(Ease.OutElastic);
                });
        }  
        _connectedNeighbors = 0;
        _facingConnectionCount = 0;
    }

}