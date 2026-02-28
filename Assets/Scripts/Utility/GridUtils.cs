using System;
using UnityEngine;

namespace Dots.Utilities
{
    public static class GridUtility
    {


        public static Vector2Int WorldToGrid(float worldX, float worldY)
        {
            return new Vector2Int(Mathf.FloorToInt(worldX / BoardView.TileSize), Mathf.FloorToInt(worldY / BoardView.TileSize));
        }
        public static Vector3 GridToWorld(Vector2Int gridPos)
        {
            return new Vector3(gridPos.x, gridPos.y, 0) * BoardView.TileSize;
        }
        public static Vector2Int WorldToGrid(Vector3 worldPosition)
        {
            return WorldToGrid(worldPosition.x, worldPosition.y);
        }
      
    }
}
