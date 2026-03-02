using UnityEngine;
using System.Collections.Generic;
using System.Collections;


/// <summary>
/// Interface for grid presenter (grid management logic)
/// </summary>
public interface IBoardPresenter
{
    //Board State
    int Width { get; }
    int Height { get; }

    // Initialization
    void Initialize(LevelData levelData);
    void ClearBoard();

    // Dot management
    void MoveDot(string id, Vector2Int endPosition);
    void RemoveDot(string id);


    // Tile management
    void SpawnTile(TileModel tile);
    void RemoveTile(string tile);


    // Dot queries
    IDotPresenter GetDotAt(Vector2Int position);
    IDotPresenter GetDotAt(int x, int y);
    IDotPresenter GetDot(string id);
    T GetDotAt<T>(Vector2Int position);
    T GetDotAt<T>(int x, int y);
    T GetDot<T>(string id);
    bool DotExists(string id);


    // Tile queries
    ITilePresenter GetTileAt(Vector2Int gridPos);
    ITilePresenter GetTileAt(int x, int y);
    ITilePresenter GetTile(string id);
    List<ITilePresenter> GetAllTiles();

    // Grid queries
    List<IDotPresenter> GetDotsOnBoard();
    List<T> GetDotsOnBoard<T>() where T : DotPresenter;
    bool IsValidPosition(Vector2Int position);

    IDotPresenter SpawnDot(Dot dot);
    bool IsOnEdgeOfBoard(Vector2Int gridPosition);
    bool IsOnEdgeOfBoard(int column, int row);
    List<T> GetDotNeighbors<T>(int column, int row, bool includesDiagonals = true) where T : class;
    List<T> GetDotNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class;
    bool IsAtBottomOfBoard(Vector2Int gridPosition);
    bool IsAtBottomOfBoard(int column, int row);
}
