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
    void MoveDot(string dotId, Vector2Int endPosition);
    void ClearDot(string dotId);
    void RemoveAndDestroyDot(string dotId);

    /// <summary>
    /// Tries to clear a dot.
    /// </summary>
    /// <param name="dotId">The dot id to hit</param>
    bool TryClearDot(string dotId);

    /// <summary>
    /// Tries to hit a dot.
    /// </summary>
    /// <param name="dotId">The dot id to hit</param>
    bool TryHitDot(string dotId, out bool shouldClear);


    // Tile management
    void SpawnTile(TileModel tile);
    void RemoveTile(string tileId);


    // Dot queries
    IDotPresenter GetDotAt(Vector2Int position);
    IDotPresenter GetDotAt(int x, int y);
    IDotPresenter GetDot(string dotId);
    T GetDotAt<T>(Vector2Int position);
    T GetDotAt<T>(int x, int y);
    T GetDot<T>(string dotId);
    bool DotExists(string dotId);


    // Tile queries
    ITilePresenter GetTileAt(Vector2Int gridPos);
    ITilePresenter GetTileAt(int x, int y);
    ITilePresenter GetTile(string tileId);
    List<ITilePresenter> GetAllTiles();

    // Grid queries
    List<IDotPresenter> GetDotsOnBoard();
    List<T> GetDotsOnBoard<T>() where T : class, IPresenter;
    bool IsValidPosition(Vector2Int position);
    IDotPresenter CreateDotPresenter(DotsObject dObject);
    IDotPresenter SpawnDot(DotsObject dObject);

    void ReplaceDot(IDotPresenter oldDot, IDotPresenter newDot, System.Action onComplete = null);
    bool IsOnEdgeOfBoard(Vector2Int gridPosition);
    bool IsOnEdgeOfBoard(int column, int row);

    List<IDotPresenter> GetDotNeighbors(Vector2Int position, bool includesDiagonals = true);
    List<T> GetDotNeighbors<T>(int column, int row, bool includesDiagonals = true) where T : class;
    List<T> GetDotNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class;
    bool IsAtBottomOfBoard(Vector2Int gridPosition);
    bool IsAtBottomOfBoard(int column, int row);
    List<BoardPresenter.DotDrop> CollectGravityDrops();
    List<BoardPresenter.DotDrop> CollectRefillDrops(DotsObject[] dotsToSpawn = null);
    List<IDotPresenter> CollectPresenters(List<string> dotIds);
    List<T> CollectPresenters<T>(List<string> dotIds) where T : class, IPresenter;
}
