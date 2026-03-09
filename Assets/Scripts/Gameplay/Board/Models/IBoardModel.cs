using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardModel
{
    int Width { get; }
    int Height { get; }
    Dot[,] DotGrid { get; }
    Tile[,] TileGrid { get; }
    List<Dot> GetAllDots();
    List<Tile> GetAllTiles();
    void SpawnTile(Tile tile);
    void SpawnDot(Dot dot);
    void RemoveTile(string tileId);
    void ClearDot(string dotId);
    void MoveDot(string id, Vector2Int toPosition);
    Dot GetDotAt(Vector2Int position);
    Dot GetDotAt(int x, int y);
    Tile GetTileAt(Vector2Int position);
    Tile GetTileAt(int x, int y);
    Dot GetDot(string dotId);
    Tile GetTile(string id);
    event Action<Dot> OnDotCleared;
    event Action<Dot> OnDotSpawned;
    event Action<Tile> OnTileRemoved;
    event Action<Tile> OnTileSpawned;
    List<Dot> InitDots(LevelData level);
    List<Tile> InitTiles(LevelData level);
    bool IsValidPosition(Vector2Int position);
    void ClearBoard();
    void ReplaceDot(string dotId, Dot dot);
}