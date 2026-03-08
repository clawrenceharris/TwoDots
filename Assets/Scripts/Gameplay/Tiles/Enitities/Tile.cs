using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : IBoardEntity
{
    private readonly Dictionary<Type, ITileModel> _models = new();
    public string ID { get; private set; }
    private Vector2Int _gridPosition;
    private readonly TileType _tileType;
    public TileType TileType => _tileType;
    public Vector2Int GridPosition
    {
        get => _gridPosition;
        set => _gridPosition = value;
    }
    public Tile(TileType type, Vector2Int position)
    {
        _tileType = type;
        _gridPosition = position;
        ID = Guid.NewGuid().ToString();
    }
}