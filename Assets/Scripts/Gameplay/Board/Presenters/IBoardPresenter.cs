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

    DotPresenter CreateDotPresenter(Dot dot);
    // Tile management
    ITilePresenter SpawnTile(DotsObject dObject);
    void RemoveTile(string tileId);
    TilePresenter CreateTilePresenter(DotsObject dObject);

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

    void ReplaceDot(DotPresenter oldDot, DotPresenter newDot, System.Action onComplete = null);
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
    bool TryClear(string clearableId);
    bool TryHit(string hittableId, out bool shouldClear);
    IBoardEntity GetEntityAt(Vector2Int targetPosition);
    IBoardEntity GetEntityAt(int x, int y);
    List<T> CollectPresenters<T>(List<string> ids) where T : class, IPresenter;
    int GetBottomMostRow(int x);
}
