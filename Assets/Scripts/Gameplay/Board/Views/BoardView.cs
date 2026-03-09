using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;


/// <summary>
/// View component for grid visual representation.
/// Handles tile creation and dot view spawning.
/// </summary>
public class BoardView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _tileSize;
    public static float TileSize;
    private readonly Dictionary<string, TileView> _tileViews = new();
    private readonly Dictionary<string, DotView> _dotViews = new();
    private readonly List<GameObject> _activeConnectionSegments = new();
    private GameObject _activeDragLine;
    private IBoardModel _boardModel;


    public void Initialize(IBoardModel model)
    {
        _boardModel = model;
        TileSize = _tileSize;
       
    }
   
    public void ClearBoard()
    {
        ClearTiles();
        ClearDots();
    }

    private void ClearTiles()
    {
        foreach (var tileView in _tileViews.Values)
        {
            if (tileView != null && tileView.gameObject != null)
                Destroy(tileView.gameObject);
        }
        _tileViews.Clear();
    }

    private void ClearDots()
    {
        foreach (var dotView in _dotViews.Values)
        {
            if (dotView != null && dotView.gameObject != null)
                Destroy(dotView.gameObject);
        }
        _dotViews.Clear();
    }

    /// <summary>
    /// Release a single dot view to the pool and remove from registry. Call when a dot is removed from the board but view may be reused (e.g. for undo).
    /// </summary>
    public void ReleaseDotView(string dotId)
    {
        if (_dotViews.TryGetValue(dotId, out var dotView))
        {
            if (dotView is BombView bombView)
            {
                var poolObj = bombView.GetComponentInParent<BombPoolObject>();
                if (poolObj != null)
                {
                    PoolService.Instance.ReturnToPool<BombPool>(poolObj);
                }
                return;
            }
            Destroy(dotView.gameObject);
            _dotViews.Remove(dotId);
        }
    }

    public TileView CreateTileView(TileModel tile)
    {
        int gridX = tile.GridPosition.x;
        int gridY = tile.GridPosition.y;
        Vector3 worldPos = new Vector3(gridX, gridY, 0);

        var view = Instantiate(PrefabLibrary.Instance.FromTileType(tile.TileType), worldPos, Quaternion.identity, transform);
        view.Init(tile);
        _tileViews.TryAdd(tile.ID, view);
        return view;


    }
    public DotView CreateDotView(Dot dot)
    {
        int gridX = dot.GridPosition.x;
        int gridY = dot.GridPosition.y;
        Vector3 worldPos = new Vector3(gridX, gridY, 0) * _tileSize;

       
        DotView view = Instantiate(PrefabLibrary.Instance.FromDotType(dot.DotType), transform);
       
        view.transform.position = worldPos;
        _dotViews.TryAdd(dot.ID, view);
        return view;
    }

   
}

