
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
    void ClearTile(string tileId);
    void ClearDot(string dotId);
    void MoveDot(string id, Vector2Int toPosition);
    Dot GetDotAt(Vector2Int position);
    Dot GetDotAt(int x, int y);
    Tile GetTileAt(Vector2Int position);
    Tile GetTileAt(int x, int y);
    Dot GetDot(string dotId);
    Tile GetTile(string id);
    List<Dot> InitDots(LevelData level);
    List<Tile> InitTiles(LevelData level);
    bool IsValidPosition(Vector2Int position);
    void ClearBoard();
    void ReplaceDot(string dotId, Dot dot);
    void Initialize(LevelData level);
    bool TryPlaceDot(Dot dot, Vector2Int position);
    void PlaceDot(Dot dot, Vector2Int position);
    bool TryPlaceTile(Tile tile, Vector2Int position);
    void PlaceTile(Tile tile, Vector2Int position);

}