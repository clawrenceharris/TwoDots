using System;
using System.Collections.Generic;
using UnityEngine;

public interface IBoardModel
{
    int Width { get; }
    int Height { get; }
    Dot[,] DotGrid { get; }
    TileModel[,] TileGrid { get; }
    List<Dot> GetAllDots();
    List<TileModel> GetAllTiles();
    void SpawnTile(TileModel tile);
    void SpawnDot(Dot dot);
    void RemoveTile(string id);
    void RemoveDot(string id);
    void MoveDot(string id, Vector2Int toPosition);
    Dot GetDotAt(Vector2Int position);
    Dot GetDotAt(int x, int y);
    TileModel GetTileAt(Vector2Int position);
    TileModel GetTileAt(int x, int y);
    Dot GetDot(string id);
    TileModel GetTile(string id);
    event Action<Dot> OnDotCleared;
    event Action<Dot> OnDotSpawned;
    event Action<TileModel> OnTileRemoved;
    event Action<TileModel> OnTileSpawned;
    List<Dot> InitDots(LevelData level);
    List<TileModel> InitTiles(LevelData level);
    bool IsValidPosition(Vector2Int position);
    void ClearBoard();
}