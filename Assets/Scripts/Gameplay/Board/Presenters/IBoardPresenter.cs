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
    void Initialize(BoardView boardView, DotSpawner dotSpawner);
    void ClearBoard();
    void SetupBoard(LevelData level);
    // Dot management
    void MoveDot(string dotId, Vector2Int endPosition);
    void ClearDot(string dotId);
    void RemoveAndDestroyDot(string dotId);
    IDotPresenter SpawnDot(Dot dot);
    DotPresenter CreateDotPresenter(DotsObject dObject);


    /// <summary>
    /// Places a dot at a specific position on the board, by adding its reference to the grid and to the board model
    /// </summary>
    /// <param name="dot">The dot to place</param>
    /// <param name="gridPosition"></param>
    bool TryPlaceDot(Dot dot, Vector2Int position);

    /// <summary>
    /// Places a dot at a specific position on the board, by adding its reference to the grid and to the board model
    /// </summary>
    /// <param name="dot">The dot to place</param>
    /// <param name="position">The position</param>
    void PlaceDot(Dot dot, Vector2Int position);
    // Tile management


    /// <summary>
    /// Tries to place a tile at the given position on the board, updating both the tile grid and the board model.
    /// </summary>
    /// <param name="tile">The tile to place of move</param>
    /// <param name="position">The target grid position</param>
    /// <returns>True if the tile was moved, false otherwise</returns>
    bool TryPlaceTile(Tile tile, Vector2Int position);

    /// <summary>
    /// Places a tile at the given position on the board, updating both the tile grid and the board model.
    /// Call this when the tile and its presenter already exist, such as when moving a tile from one position to another.
    /// </summary>
    /// <param name="tile">The tile to place or move.</param>
    /// <param name="position">The target grid position for the tile.</param>
    void PlaceTile(Tile tile, Vector2Int position);
    // Dot queries
    DotPresenter GetDotAt(Vector2Int position);
    DotPresenter GetDotAt(int x, int y);
    DotPresenter GetDot(string dotId);
    T GetDotAt<T>(Vector2Int position);
    T GetDotAt<T>(int x, int y);
    T GetDot<T>(string dotId);
    bool DotExists(string dotId, out IDotPresenter presenter);


    // Tile queries
    TilePresenter GetTileAt(Vector2Int gridPos);
    TilePresenter GetTileAt(int x, int y);
    TilePresenter GetTile(string tileId);
    T GetTile<T>(string tileId);
    List<TilePresenter> GetTilesOnBoard();
    List<T> GetTilesOnBoard<T>() where T : class, IPresenter;
    bool TileExists(string id, out ITilePresenter presenter);

    // Board queries
    List<DotPresenter> GetDotsOnBoard();
    List<T> GetDotsOnBoard<T>() where T : class, IPresenter;
    bool IsValidPosition(Vector2Int position);
    void ReplaceDot(Dot oldDot, Dot newDot, System.Action onComplete = null);
    bool IsOnEdgeOfBoard(Vector2Int gridPosition);
    bool IsOnEdgeOfBoard(int column, int row);
    List<IDotPresenter> GetDotNeighbors(Vector2Int position, bool includesDiagonals = true);
    List<T> GetDotNeighbors<T>(int column, int row, bool includesDiagonals = true) where T : class;
    List<T> GetDotNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class;
    List<T> GetNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class;
    List<ITilePresenter> GetTileNeighbors(Vector2Int position, bool includesDiagonals = true);
    List<T> GetTileNeighbors<T>(int x, int y, bool includesDiagonals = true) where T : class;
    List<T> GetTileNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class;
    List<IBoardEntity> GetNeighbors(Vector2Int position, bool includesDiagonals = true);
    bool IsAtBottomOfBoard(Vector2Int gridPosition);
    bool IsAtBottomOfBoard(int column, int row);

    EntityPresenter GetEntity(string id);
    List<BoardPresenter.DotDrop> CollectGravityDrops();
    List<BoardPresenter.DotDrop> CollectRefillDrops(DotsObject[] dotsToSpawn = null);
    List<T> CollectDotPresenters<T>(List<string> dotIds) where T : class, IPresenter;
    List<T> CollectTilePresenters<T>(List<string> tileIds) where T : class, IPresenter;
    bool TryHit(string hittableId, out bool shouldClear);
    IBoardEntity GetEntityAt(Vector2Int targetPosition);
    IBoardEntity GetEntityAt(int x, int y);
    List<T> CollectPresenters<T>(List<string> ids) where T : class, IPresenter;
    int GetBottomMostRow(int x);
    void ClearEntity(string entityId);
}
