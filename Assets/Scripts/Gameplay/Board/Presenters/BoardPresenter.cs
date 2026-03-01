
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;


public class BoardPresenter : MonoBehaviour, IBoardPresenter
{
    // Board State
    private IBoardModel _boardModel;


    public int Width => _boardModel.Width;

    public int Height => _boardModel.Height;



    // Board Events

    /// <summary>
    /// Called when the board presenter, view and model is initialized
    /// </summary>
    public static event Action<IBoardPresenter> OnBoardInitialized;
    /// <summary>
    /// Called when the board is fully initialized and initial spawn animations have completed
    /// </summary>
    public static event Action<IBoardPresenter> OnBoardSetupComplete;
    private DotSpawner _dotSpawner;
    // Presenters
    private readonly Dictionary<string, IDotPresenter> _dotPresenters = new();
    private readonly Dictionary<string, ITilePresenter> _tilePresenters = new();

    private BoardView _boardView;
    private DotsObject[] _dotsToSpawn;

    public static float Offset { get; private set; } = 2.5f;

    public readonly struct DotDrop
    {
        public IDotPresenter Presenter { get; }
        public int TargetRow { get; }

        public DotDrop(IDotPresenter presenter, int targetRow)
        {
            Presenter = presenter;
            TargetRow = targetRow;
        }
    }

    #region Board Lifecycle
    private void Awake()
    {
        _boardView = FindFirstObjectByType<BoardView>();
        _dotSpawner = FindFirstObjectByType<DotSpawner>();
    }


   

    #endregion


    #region  Initialization/Setup
    public void Initialize(LevelData level)
    {
        ClearBoard();
        _boardModel = new BoardModel(level);

        if (_boardView == null)
        {
            _boardView = new GameObject("BoardView").AddComponent<BoardView>();
            _boardView.transform.SetParent(transform);

        }
        _boardView.Initialize(_boardModel);
        _dotsToSpawn = level.dotsToSpawn;
        OnBoardInitialized?.Invoke(this);
        SetupBoard(level);

    }
   
    /// <summary>
    /// Gets a board mechanic tile at the specified column and row
    /// </summary>
    /// <typeparam name="T">The desired type that the desired board mechanic inherits</typeparam>
    /// <param name="col">The column of the desired board mechanic tile</param>
    /// <param name="row">The row of the desired board mechanic tile</param>
    /// <returns>A board mechanic tile at the specified column and row or null if not found</returns>
    public T GetBoardMechanicTileAt<T>(int col, int row)
    {
        if (col >= 0 && col < Width && row >= 0 && row < Height)
        {
            TileModel tile = _boardModel.GetTileAt(col, row);
            if (tile is T t && tile.TileType.IsBoardMechanicTile())
                return t;
        }
        return default;
    }
    

    
    private IDotPresenter InitRandomDot(int col, int row, DotsObject[] dotsToSpawn)
    {

        DotsObject data = _dotSpawner.GetRandomDot(dotsToSpawn.ToList());
        data.Col = col;
        data.Row = row;

        Dot dot = DotFactory.CreateDot(data);
        DotView view = _boardView.CreateDotView(dot);
        IDotPresenter presenter = DotFactory.CreateDotPresenter(dot, view);
        _dotPresenters.Add(dot.ID, presenter);
        presenter.OnDotCleared += RemoveDotPresenter;
        _boardModel.SpawnDot(dot);
        return presenter;
    }


    public void SetupBoard(LevelData level)
    {
        foreach (var dot in level.dotsOnBoard)
        {
            SpawnDot(dot);
        }
        FillBoard(level.initDotsToSpawn.Length > 0 ? level.initDotsToSpawn : level.dotsToSpawn);
        OnBoardSetupComplete?.Invoke(this);


    }
    #endregion


   
   




    #region  Board Management
    /// <summary>
    /// Clears the board by clearing the tile and dot presenters and the board model.
    /// </summary>
    public void ClearBoard()
    {
        _tilePresenters?.Clear();
        _dotPresenters?.Clear();
        _boardModel?.ClearBoard();
    }
    #endregion

    #region Board Queries

    public List<T> GetDotsAtRow<T>(int row)
    {
        List<T> dots = new();
        for (int col = 0; col < Width; col++)
        {
            T dot = GetDotAt<T>(col, row);
            if (dot != null)
            {
                dots.Add(dot);


            }
        }
        return dots;

    }

    public List<IDotPresenter> GetDotsBetween(Vector2Int gridPosition1, Vector2Int gridPosition2)
    {
        var dots = new List<IDotPresenter>();
        for (int x = gridPosition1.x; x <= gridPosition2.x; x++)
        {
            for (int y = gridPosition1.y; y <= gridPosition2.y; y++)
            {
                var dot = GetDotAt<IDotPresenter>(x, y);
                dots.Add(dot);
            }
        }
        return dots;
    }

    /// <summary>
    /// Returns all playable dots on the board.
    /// </summary>
    /// <returns>
    /// List of all playable dots presenters on the board.
    /// </returns>
    public List<IDotPresenter> GetDotsOnBoard() => _boardModel.GetAllDots().Select(b => _dotPresenters[b.ID]).ToList();

    public List<T> GetDotNeighbors<T>(int x, int y, bool includesDiagonals = true) where T : class
    {
        var neighbors = new List<T>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                if (!includesDiagonals && i != 0 && j != 0) continue;
                var neighbor = GetDotAt<T>(x + i, y + j);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor as T);
                }
            }
        }
        return neighbors;
    }
    public List<T> GetDotNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class
    {
        return GetDotNeighbors<T>(position.x, position.y, includesDiagonals);
    }
    public void PlaceDot(Dot dot, int x, int y)
    {
        dot.GridPosition = new Vector2Int(x, y);
        _boardModel.SpawnDot(dot);
    }
    public void PlaceDot(Dot dot, Vector2Int position)
    {
        PlaceDot(dot, position.x, position.y);
    }

    public bool IsOnEdgeOfBoard(Vector2Int position)
    {
        return position.x == 0 || position.x == Width - 1 || position.y == 0 || position.y == Height - 1;
    }
    public IDotPresenter GetDotAt(Vector2Int position) => GetDotAt<IDotPresenter>(position.x, position.y);
    public IDotPresenter GetDotAt(int x, int y) => GetDotAt<IDotPresenter>(x, y);
    public T GetDot<T>(string id)
    {

        if (_dotPresenters.TryGetValue(id, out var presenter) && presenter is T t)
        {
            return t;
        }
        return default;
    }
    public T GetDotAt<T>(Vector2Int position) => GetDotAt<T>(position.x, position.y);

    public T GetDotAt<T>(int x, int y)
    {

        var dot = _boardModel.GetDotAt(x, y);
        if (dot != null)
        {
            if (_dotPresenters.TryGetValue(dot.ID, out var presenter) && presenter is T t)
            {
                return t;
            }
        }
        return default;
    }
    
    public IDotPresenter GetDot(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (!_dotPresenters.TryGetValue(id, out var p))
        {
            Debug.LogWarning($"[BoardPresenter] Dot {id} does not exist");
            return null;

        }
        return p; ;

    }

    public List<IDotPresenter> CollectPresenters(IEnumerable<string> dotIds)
    {
        var presenters = new List<IDotPresenter>();
        if (dotIds == null) return presenters;
        var seen = new HashSet<string>();

        foreach (var id in dotIds)
        {
            if (!seen.Add(id)) continue;
            var presenter = GetDot(id);
            if (presenter != null)
                presenters.Add(presenter);
        }

        return presenters
            .OrderBy(p => p.Dot.GridPosition.y)
            .ThenBy(p => p.Dot.GridPosition.x)
            .ToList();
    }
    public bool DotExists(string id)
    {
        if (!_dotPresenters.TryGetValue(id, out var _))
        {
            Debug.LogError($"[BoardPresenter] Dot {id} does not exist");
            return false;
        }
        if (_boardModel.GetDot(id) == null)
        {
            Debug.LogError($"[BoardPresenter] Dot {id} does not exist");
            return false;
        }

        return true;
    }
    #endregion




    #region Tile Queries

    public List<ITilePresenter> GetAllTiles() => _tilePresenters.Values.ToList();

    public ITilePresenter GetTileAt(Vector2Int position) => GetTileAt(position.x, position.y);

    public ITilePresenter GetTileAt(int x, int y)
    {

        var tile = _boardModel.GetTileAt(x, y);
        if (tile != null)
        {
            if (_tilePresenters.TryGetValue(tile.ID, out var presenter))
            {
                return presenter;
            }
        }
        return null;
    }

    #endregion

    #region  Dot Management
    

    public void MoveDot(string id, Vector2Int endPosition)
    {
        _boardModel.MoveDot(id, endPosition);
    }

    public IDotPresenter SpawnDot(Dot dot)
    {
        if (_boardView == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return null;
        }


        DotView view = _boardView.CreateDotView(dot);
        var presenter = DotFactory.CreateDotPresenter(dot, view);
        _dotPresenters.Add(dot.ID, presenter);
        presenter.OnDotCleared += RemoveDotPresenter;
        _boardModel.SpawnDot(dot);
        return presenter;
    }
     public IDotPresenter SpawnDot(DotsObject dObject)
    {
        if (_boardView == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return null;
        }

        Dot dot = DotFactory.CreateDot(dObject);
        DotView view = _boardView.CreateDotView(dot);
        var presenter = DotFactory.CreateDotPresenter(dot, view);
        _dotPresenters.Add(dot.ID, presenter);
        presenter.OnDotCleared += RemoveDotPresenter;
        _boardModel.SpawnDot(dot);
        return presenter;
    }


   

    public void RemoveDot(string id)
    {

        _boardModel.RemoveDot(id);


    }
    /// <summary>
    /// Called after remove animation completes. Releases the dot view to the pool and moves the presenter
    /// </summary>
    private void RemoveDotPresenter(IDotPresenter presenter)
    {
        // Release the view to the pool (so it can be reused by ID on undo)
        if (_boardView != null)
            _boardView.ReleaseDotView(presenter.Dot.ID);

        _dotPresenters.Remove(presenter.Dot.ID);
        presenter.OnDotCleared -= RemoveDotPresenter;
    }

    #endregion

    #region Tile Management

    public void RemoveTile(string id)
    {
        _boardModel.RemoveTile(id);

    }
    private void RemoveTilePresenter(ITilePresenter presenter)
    {
        if (!_tilePresenters.TryGetValue(presenter.Model.ID, out var _))
        {
            Debug.LogError($"[BoardPresenter] RemoveTilePresenter: no presenter for {presenter.Model.ID}");
            return;
        }

        Vector2Int removedPosition = presenter.Model.GridPosition;
        _tilePresenters.Remove(presenter.Model.ID);

        // Update neighbors after removing this tile
        UpdateNeighborTileSprites(removedPosition);
    }

    public void SpawnTile(TileModel tile)
    {
        if (_boardView == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return;
        }
        var view = _boardView.CreateTileView(tile);

        var presenter = TileFactory.CreateTilePresenter(tile, view);
        _tilePresenters.Add(tile.ID, presenter);
        _boardModel.SpawnTile(tile);

        UpdateNeighborTileSprites(tile.GridPosition);
    }

    public ITilePresenter GetTile(string id) => _tilePresenters.TryGetValue(id, out var presenter) ? presenter : null;

    /// <summary>
    /// Updates the sprites of tiles neighboring the given position.
    /// Called when a tile is spawned or removed to refresh neighbor sprites.
    /// </summary>
    private void UpdateNeighborTileSprites(Vector2Int position)
    {
        if (_boardModel == null) return;

        // Check all 4 cardinal directions
        Vector2Int[] offsets = new Vector2Int[]
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var offset in offsets)
        {
            Vector2Int neighborPos = position + offset;

            if (_boardModel.IsValidPosition(neighborPos))
            {
                TileModel neighborTile = _boardModel.GetTileAt(neighborPos);
                if (neighborTile != null)
                {
                    ITilePresenter neighborPresenter = GetTileAt(neighborPos);
                    if (neighborPresenter != null && neighborPresenter.View != null)
                    {
                        // neighborPresenter.View.UpdateTileSprite(_boardModel);
                    }
                }
            }
        }
    }

    public bool IsValidPosition(Vector2Int position) => _boardModel.IsValidPosition(position);




    #endregion

    public override string ToString()
    {
        return _boardModel.ToString();
    }




    public bool HasAny<T>()
    {
        if (typeof(T) == typeof(Dot) || typeof(T).IsSubclassOf(typeof(Dot)))
        {
            foreach (Dot dot in _boardModel.DotGrid)
            {
                if (dot is T)
                {
                    return true;
                }
            }
        }

        else if (typeof(T) == typeof(TileModel) || typeof(T).IsSubclassOf(typeof(TileModel)))
        {
            foreach (TileModel tile in _boardModel.TileGrid)
            {
                if (tile is T)
                {
                    return true;
                }
            }
        }
        return false;
    }

    
    public static Vector2 GetPosition(int col, int row)
    {
        return new Vector2(col, row) * Offset;
    }

    
    


     public bool IsOnEdgeOfBoard(int col, int row)
    {
        return IsAtBottomOfBoard(col, row) || IsAtLeftOfBoard(col, row) || IsAtRightOfBoard(col, row) || IsAtTopOfBoard(col, row);
    }
     public bool FillBoard(DotsObject[] dotsToSpawn = null)
    {
        var drops = CollectRefillDrops(dotsToSpawn);
        foreach (var drop in drops)
        {
            if (drop.Presenter == null) continue;
            drop.Presenter.Drop(drop.TargetRow);
        }
        return drops.Count > 0;
    }


    public bool CollapseColumn()
    {
        var drops = CollectGravityDrops();
        foreach (var drop in drops)
        {
            if (drop.Presenter == null) continue;
            drop.Presenter.Drop(drop.TargetRow);
        }
        return drops.Count > 0;
    }
    public bool CollapseColumn(out List<DotDrop> drops)
    {
        drops = new List<DotDrop>();

        bool dotsDropped = false;
        for (int col = 0; col < Width; col++)
        {

            for (int row = Height - 1; row >= 0; row--)
            {
                ITilePresenter tile = GetTileAt(col, row);
                if (tile != null && tile.Model.TileType.IsBlockable())
                {
                    break;

                }
                if (_boardModel.DotGrid[col, row] == null)
                {
                    if(tile == null || !tile.Model.TileType.IsBoardMechanicTile()){
                        for (int k = row + 1; k < Height; k++)
                        {
                            IDotPresenter dot = GetDotAt(col, k);
                            if (dot != null)
                            {
                                _boardModel.MoveDot(dot.Dot.ID, new Vector2Int(col, row));
                                drops.Add(new DotDrop(dot, row));
                                dotsDropped = true;
                                break;
                            }

                        }
                    }

                    
                }


            }


        }
        return dotsDropped;
    }
    public List<DotDrop> CollectGravityDrops()
    {
        var allDrops = new List<DotDrop>();
        bool dotsDropped;
        do
        {
            dotsDropped = CollapseColumn(out var drops);
            allDrops.AddRange(drops);



        } while (dotsDropped);
        

        return allDrops;
    }

    public List<DotDrop> CollectRefillDrops(DotsObject[] dotsToSpawn = null)
    {
        var drops = new List<DotDrop>();
        for (int col = 0; col < Width; col++)
        {
            for (int row = Height - 1; row >= 0; row--)
            {
                ITilePresenter tile = GetTileAt(col, row);
                if (tile != null && tile.Model.TileType.IsBlockable())
                {
                    break;
                }
                if (GetDotAt(col, row) == null)
                {
                    if (tile == null || !tile.Model.TileType.IsBoardMechanicTile())
                    {
                        IDotPresenter dot = InitRandomDot(col, row, dotsToSpawn ?? _dotsToSpawn);
                        drops.Add(new DotDrop(dot, row));
                    }
                }
            }
        }

        return drops;
    }

    /// <summary>
    /// Checks if the dot is at the far bottom of the board meaning no dots are under it.
    /// </summary>
    /// <param name="col"> the column of the dot to check</param>
    /// <param name="row">the row of the dot to check</param>
    /// <returns>Whether or not the dot is at the far bottom</returns>
    public bool IsAtBottomOfBoard(int col, int row)
    {

        for (int i = row - 1; i >= 0; i--)
        {
            IDotPresenter dot = GetDotAt(col, i);
            if (dot != null)
            {
                return false;
            }
        }

        return true;

    }

    /// <summary>
    /// Checks if the dot is on the far left of the board meaning no dots are to the left of it. 
    /// </summary>
    /// <param name="col"> the column of the dot to check</param>
    /// <param name="row">the row of the dot to check</param>
    /// <returns>Whether or not the dot is on the far left.</returns>
    public bool IsAtLeftOfBoard(int col, int row)
    {

        for (int i = col - 1; i >= 0; i--)
        {
            IDotPresenter dot = GetDotAt(i, row);

            if (dot != null)
            {
                return false;
            }
        }
        return true;

    }

    /// <summary>
    /// Checks if the dot is on the far right of the board meaning no dots are to the right of it. 
    /// </summary>
    /// <param name="col"> the column of the dot to check</param>
    /// <param name="row">the row of the dot to check</param>
    /// <returns>Whether or not the dot is on the far right.</returns>
    public bool IsAtRightOfBoard(int col, int row)
    {

        for (int i = col + 1; i < Width; i++)
        {
            IDotPresenter dot = GetDotAt(i, row);

            if (dot != null)
            {
                return false;
            }
        }

        return true;

    }

    /// <summary>
    /// Checks if the dot is on the very top of the board meaning no dots are above it. 
    /// </summary>
    /// <param name="col"> the column of the dot to check</param>
    /// <param name="row">the row of the dot to check</param>
    /// <returns>Whether or not the dot is on the very top.</returns>
    public bool IsAtTopOfBoard(int col, int row)
    {

        for (int i = row + 1; i < Height; i++)
        {
            IDotPresenter dot = GetDotAt(col, i);

            if (dot != null)
            {
                return false;
            }
        }

        return true;

    }
    public IDotPresenter GetBottomMostDot(int col)
    {
        for (int row = 0; row < Height; row++)
        {
            IDotPresenter dot = GetDotAt(col, row);
            if (dot != null)
            {
                return dot;
            }
        }
        return null;
    }

    public bool IsAtBottomOfBoard(Vector2Int gridPosition)
    {
        return IsAtBottomOfBoard(gridPosition.x, gridPosition.y);
    }
}

