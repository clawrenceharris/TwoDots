using System;
using UnityEngine;

public class TileModel : ITileModel
{
    public TileType TileType { get; private set; }
    public Vector2Int GridPosition { get; private set; }
    public string ID { get; private set; }
    public TileModel(Vector2Int gridPosition, TileType tileType)
    {
        GridPosition = gridPosition;
        TileType = tileType;
        ID = Guid.NewGuid().ToString();
    }
    public override string ToString()
    {
        return $"{TileType} Tile - {GridPosition}";
    }
}