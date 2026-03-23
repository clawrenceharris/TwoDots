
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Dots.Utilities;
using UnityEngine;


public class BoardPresenter : IBoardPresenter
{
    // Board State
    private IBoardModel _model;


    public int Width => _model.Width;

    public int Height => _model.Height;



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
    private readonly Dictionary<string, DotPresenter> _dotPresenters = new();
    private readonly Dictionary<string, TilePresenter> _tilePresenters = new();

    private BoardView _view;
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
   



    #endregion


    #region  Initialization/Setup
    public void Initialize(BoardView boardView, DotSpawner dotSpawner)
    {
        _model = new BoardModel();
        _view = boardView;
        _dotSpawner = dotSpawner;

        OnBoardInitialized?.Invoke(this);

    }

    public void SetupBoard(LevelData level)
    {
        ClearBoard();
        _model.Initialize(level);
        _view.Initialize(_model);

        var dots = _model.InitDots(level);
        var tiles = _model.InitTiles(level);
        _dotsToSpawn = level.dotsToSpawn;
       
        foreach (var tile in level.tilesOnBoard)
        {
            SpawnTile(TileFactory.CreateTile(tile));
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
        _model?.ClearBoard();
    }
    

    #endregion

    #region Board Queries

    public IBoardEntity GetEntityAt(Vector2Int targetPosition) => GetEntityAt(targetPosition.x, targetPosition.y);
    public IBoardEntity GetEntityAt(int x, int y)
    {
        var dot = GetDotAt(x, y);
        if(dot != null)
        {
            return dot.Entity;
        }
        var tile = GetTileAt(x, y);
        if(tile != null)
        {
            return tile.Entity;
        }
        return null;
    }
    public List<IBoardEntity> GetNeighbors(Vector2Int position, bool includesDiagonals = true)
    {
        List<IBoardEntity> neighbors = new();
        var offsets = new List<Vector2Int>
        {
            Vector2Int.left,
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down
        };
        if (includesDiagonals)
        {
            offsets.Add(Vector2Int.up + Vector2Int.left);
            offsets.Add(Vector2Int.up + Vector2Int.right);
            offsets.Add(Vector2Int.down + Vector2Int.left);
            offsets.Add(Vector2Int.down + Vector2Int.right);
        }
        foreach (var offset in offsets)
        {
            var dotNeighbor = GetDotAt(position + offset);
            if (dotNeighbor != null)
            {
                neighbors.Add(dotNeighbor.Dot);
            }
            var tileNeighbor = GetTileAt(position + offset);
            if (tileNeighbor != null)
            {
                neighbors.Add(tileNeighbor.Tile);
            }
        }
        return neighbors;
    }

    
    public List<T> GetNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class
    {
        List<T> neighbors = new();
        var dotNeighbors = GetDotNeighbors<T>(position, includesDiagonals);
        var tileNeighbors = GetTileNeighbors<T>(position, includesDiagonals);
        neighbors.AddRange(dotNeighbors);
        neighbors.AddRange(tileNeighbors);
        return neighbors;
    }
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
    


    public List<T> CollectTilePresenters<T>(List<string> tileIds)
    where T : class, IPresenter
    {
        var presenters = new List<T>();
        if (tileIds == null) return presenters;
        var seen = new HashSet<string>();

        foreach (var id in tileIds)
        {
            if (!seen.Add(id)) continue;
            var presenter = GetTile(id);
            if (presenter != null && presenter.TryGetPresenter(out T tPresenter))
            {
                presenters.Add(tPresenter);
            }
           
        }
        return presenters;
    }
    public List<T> CollectDotPresenters<T>(List<string> dotIds)
    where T : class, IPresenter
    {
        var presenters = new List<T>();
        if (dotIds == null) return presenters;
        var seen = new HashSet<string>();

        foreach (var id in dotIds)
        {
            if (!seen.Add(id)) continue;
            var presenter = GetDot(id);
            if (presenter != null && presenter.TryGetPresenter(out T tPresenter))
            {
                presenters.Add(tPresenter);
            }
           
        }
        return presenters;

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
    public List<DotPresenter> GetDotsOnBoard() => _dotPresenters.Values.ToList();
    public List<T> GetDotsOnBoard<T>() where T : class, IPresenter
    {
        var presenters = new List<T>();
       foreach (var dot in _dotPresenters.Values)
        {
            if (dot.TryGetPresenter(out T tPresenter))
            {
                presenters.Add(tPresenter);
            }
        }
       return presenters;
    }
    public List<IDotPresenter> GetDotNeighbors(Vector2Int position, bool includesDiagonals = true)
    {
        return GetDotNeighbors<IDotPresenter>(position, includesDiagonals);
    }
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
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }
    public List<T> GetDotNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class
    {
        return GetDotNeighbors<T>(position.x, position.y, includesDiagonals);
    }

    public bool TryPlaceDot(Dot dot, Vector2Int position)
    {
        return _model.TryPlaceDot(dot, position);
    }
    
    public void PlaceDot(Dot dot, Vector2Int position)
    {
        _model.PlaceDot(dot, position);

    }

    public bool IsOnEdgeOfBoard(Vector2Int position)
    {
        return position.x == 0 || position.x == Width - 1 || position.y == 0 || position.y == Height - 1;
    }
    public DotPresenter GetDotAt(Vector2Int position) => GetDotAt(position.x, position.y);
    public DotPresenter GetDotAt(int x, int y)
    {
        var dot = _model.GetDotAt(x, y);
        if (dot != null)
        {
            if (_dotPresenters.TryGetValue(dot.ID, out var presenter))
            {
                return presenter;
            }
        }
        return null;
    }
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

        var dot = _model.GetDotAt(x, y);
        if (dot != null)
        {
            if (_dotPresenters.TryGetValue(dot.ID, out var presenter) && presenter is T t)
            {
                return t;
            }
        }
        return default;
    }

    public DotPresenter GetDot(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (!_dotPresenters.TryGetValue(id, out var p))
        {
            return null;

        }
        return p; ;

    }

    public bool DotExists(string id, out IDotPresenter presenter)
    {
        presenter = null;
        if (!_dotPresenters.TryGetValue(id, out var _))
        {
            return false;
        }
        if (_model.GetDot(id) == null)
        {
            return false;
        }

        presenter = _dotPresenters[id];
        return true;
    }
    #endregion




    #region Tile Queries

    public List<TilePresenter> GetTilesOnBoard() => _tilePresenters.Values.ToList();
    public List<T> GetTilesOnBoard<T>() where T : class, IPresenter
    {
        var presenters = new List<T>();
        foreach (var tile in _tilePresenters.Values)
        {
            if (tile.TryGetPresenter(out T tPresenter))
            {
                presenters.Add(tPresenter);
            }
        }
        return presenters;
    }
    public TilePresenter GetTile(string id) => _tilePresenters.TryGetValue(id, out var presenter) ? presenter : null;
    public T GetTile<T>(string id)
    {
        if (_tilePresenters.TryGetValue(id, out var presenter) && presenter is T t)
        {
            return t;
        }
        return default;
    }
    public T GetTileAt<T>(Vector2Int position) => GetTileAt<T>(position.x, position.y);
    public T GetTileAt<T>(int x, int y)
    {
         var tile = _model.GetTileAt(x, y);
        if (tile != null)
        {
            if (_tilePresenters.TryGetValue(tile.ID, out var presenter) && presenter is T t)
            {
                return t;
            }
        }
        return default;
    }
    public TilePresenter GetTileAt(Vector2Int position) => GetTileAt(position.x, position.y);

    public TilePresenter GetTileAt(int x, int y)
    {

        var tile = _model.GetTileAt(x, y);
        if (tile != null)
        {
            if (_tilePresenters.TryGetValue(tile.ID, out var presenter))
            {
                return presenter;
            }
        }
        return null;
    }
    public List<ITilePresenter> GetTileNeighbors(Vector2Int position, bool includesDiagonals = true)
    {
        return GetTileNeighbors<ITilePresenter>(position, includesDiagonals);
    }
    public List<T> GetTileNeighbors<T>(Vector2Int position, bool includesDiagonals = true) where T : class
    {
        return GetTileNeighbors<T>(position.x, position.y, includesDiagonals);
    }

    public List<T> GetTileNeighbors<T>(int x, int y, bool includesDiagonals = true) where T : class
    {
        var neighbors = new List<T>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) continue;
                if (!includesDiagonals && i != 0 && j != 0) continue;
                var neighbor = GetTileAt<T>(x + i, y + j);
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }
    public bool TileExists(string id, out ITilePresenter presenter)
    {
        presenter = null;
        if (!_tilePresenters.TryGetValue(id, out var _))
        {
            Debug.LogError($"[BoardPresenter] Tile {id} does not exist");
            return false;
        }
        if (_model.GetTile(id) == null)
        {
            Debug.LogError($"[BoardPresenter] Tile {id} does not exist");
            return false;
        }
        presenter = _tilePresenters[id];
        return true;
    }

    #endregion



    #region  Dot Management

    public void MoveDot(string id, Vector2Int endPosition)
    {
        _model.MoveDot(id, endPosition);
    }


    
    
    public EntityPresenter GetEntity(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (_dotPresenters.TryGetValue(id, out DotPresenter dp))
        {
            return dp;
        }
        if (_tilePresenters.TryGetValue(id, out var tp))
        {
            return tp;
        }
        return null;
    }
    public void ClearEntity(string entityId)
    {      
        TryClearEntity(entityId);
    }

    public bool TryClearEntity(string entityId)
    {
        EntityPresenter entity = GetEntity(entityId);
        if (entity == null)
        {
            return false;
        }
        if (entity.Entity is Dot dot)
        {
            _model.ClearDot(dot.ID);
            return true;
        }
        else if (entity.Entity is Tile tile)
        {
            _model.ClearTile(tile.ID);
            return true;
        }
        return false;
    }

    
    
    
    public bool TryHit(string hittableId, out bool shouldClear)
    {
        var hittable = GetEntity(hittableId);
        shouldClear = false;
        if (hittable == null)
        {
            return false;
        }
        if (hittable.Entity.TryGetModel(out Hittable hittableModel) && hittableModel.ShouldHit())
        {
            shouldClear = hittableModel.ShouldClearAfterHit();
            hittableModel.Hit();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Creates a dot presenter from a Dot.
    /// </summary>
    /// <param name="dObject"></param>
    /// <returns></returns>
    public DotPresenter CreateDotPresenter(Dot dot)
    {
        if (_view == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return null;
        }


        var view = _view.CreateDotView(dot);
        
        var presenter = DotFactory.CreateDotPresenter(dot, view, this);
        presenter.Initialize();
        return presenter;
    }
     /// <summary>
    /// Creates a dot presenter from a DotsObject.
    /// </summary>
    /// <param name="dObject"></param>
    /// <returns></returns>
    public DotPresenter CreateDotPresenter(DotsObject dObject)
    {
        if (_view == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return null;
        }

        var dot = DotFactory.CreateDot(dObject);
        var view = _view.CreateDotView(dot);
        var presenter = DotFactory.CreateDotPresenter(dot, view, this);
        presenter.Initialize();
        return presenter;
    }
    

    private IDotPresenter SpawnRandomDotAt(Vector2Int position, DotsObject[] dotsToSpawn)
    {

        DotsObject dot = _dotSpawner.GetRandomDot(dotsToSpawn.ToList());
        dot.Col = position.x;
        dot.Row = position.y;
        var presenter = SpawnDot(DotFactory.CreateDot(dot));
        return presenter;
    }

    

    
    public IDotPresenter SpawnDot(Dot dot)
    {
        if (_view == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return null;
        }
        if (!_model.TryPlaceDot(dot, dot.GridPosition))
        {
            Debug.LogError($"[BoardPresenter] Failed to spawn dot {dot.ID} at {dot.GridPosition}");
            return null;
        }

        var presenter = CreateDotPresenter(dot);
        _dotPresenters.Add(dot.ID, presenter);
        
        return presenter;
    }


    

    public void ReplaceDot(Dot oldDot, Dot newDot, Action onComplete = null)
    {
        var oldDotPresenter = GetDot(oldDot.ID);
        var newDotPresenter = GetDot(newDot.ID);
        _model.ReplaceDot(oldDot.ID, newDot);
        _view.ReleaseDotView(oldDot.ID);
        if (newDotPresenter == null)
        {
            Debug.Log($"[BoardPresenter] Creating new dot presenter for {newDot.ID}");
            newDotPresenter = CreateDotPresenter(newDot);
        }
        newDotPresenter.DotView.transform.position = oldDotPresenter.DotView.transform.position;
        newDotPresenter.Dot.GridPosition = oldDotPresenter.Dot.GridPosition;
        newDotPresenter.DotView.Init(newDotPresenter.Dot);

        _dotPresenters.Remove(oldDot.ID);
        _dotPresenters.Add(newDot.ID, newDotPresenter);
        onComplete?.Invoke();
    
       
    }
    public void RemoveAndDestroyDot(string id)
    {
        _model.ClearDot(id);
        _view.ReleaseDotView(id);
        _dotPresenters.Remove(id);
    }

    public void ClearDot(string id)
    {
        _model.ClearDot(id);

    }
    
    
    private void RemoveDotPresenter(string dotId)
    {

        if (!_tilePresenters.TryGetValue(dotId, out var presenter))
        {
            Debug.LogError($"[BoardPresenter] RemoveTilePresenter: no presenter for {presenter.Tile.ID}");
            return;
        }

        _tilePresenters.Remove(presenter.Tile.ID);

    
    }

    #endregion

    #region Tile Management

    
    public void RemoveTile(string id)
    {
        _model.ClearTile(id);

    }
    private void RemoveTilePresenter( string tileId)
    {
        if (!_tilePresenters.TryGetValue(tileId, out var presenter))
        {
            Debug.LogError($"[BoardPresenter] RemoveTilePresenter: no presenter for {presenter.Tile.ID}");
            return;
        }

        Vector2Int removedPosition = presenter.Tile.GridPosition;
        _tilePresenters.Remove(presenter.Tile.ID);

        // Update neighbors after removing this tile
        UpdateNeighborTileSprites(removedPosition);
    }
    /// <summary>
    /// Creates and initializes a tile presenter from a DotsObject.
    /// </summary>
    /// <param name="dObject"></param>
    /// <returns>The tile presenter that was created</returns>
    public TilePresenter CreateTilePresenter(Tile tile)
    {
        if (_view == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return null;
        }


        var view = _view.CreateTileView(tile);
        var presenter = TileFactory.CreateTilePresenter(tile, view, this);
        presenter.Initialize();
        return presenter;
    }
    public bool TryPlaceTile(Tile tile, Vector2Int position)
    {
        return _model.TryPlaceTile(tile, position);
    }
    public void PlaceTile(Tile tile, Vector2Int position)
    {
        _model.PlaceTile(tile, position);
    }
    public ITilePresenter SpawnTile(Tile tile)
    {
        if (_view == null)
        {
            Debug.LogError("[BoardPresenter] BoardView is null");
            return null;
        }
        if(!_model.TryPlaceTile(tile, tile.GridPosition))
        {
            Debug.LogError($"[BoardPresenter] Failed to spawn tile {tile.ID} at {tile.GridPosition}");
            return null;
        }   
        TilePresenter presenter = CreateTilePresenter(tile);
        _tilePresenters.Add(presenter.Tile.ID, presenter);
       

        UpdateNeighborTileSprites(presenter.Tile.GridPosition);
        return presenter;

    }


    /// <summary>
    /// Updates the sprites of tiles neighboring the given position.
    /// Called when a tile is spawned or removed to refresh neighbor sprites.
    /// </summary>
    private void UpdateNeighborTileSprites(Vector2Int position)
    {
        if (_model == null) return;

        // Check all 4 cardinal directions
        Vector2Int[] offsets = new Vector2Int[]
        {
            Vector2Int.zero,
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var offset in offsets)
        {
            Vector2Int neighborPos = position + offset;

            if (_model.IsValidPosition(neighborPos))
            {
                Tile neighborTile = _model.GetTileAt(neighborPos);
                if (neighborTile != null && TileExists(neighborTile.ID, out var neighborPresenter))
                {
                    neighborPresenter.TileView.UpdateTileSprite(this);
                    
                }
            }
        }
    }

    public bool IsValidPosition(Vector2Int position) => _model.IsValidPosition(position);




    #endregion

   



    public bool HasAny<T>()
    {
        if (typeof(T) == typeof(Dot) || typeof(T).IsSubclassOf(typeof(Dot)))
        {
            foreach (Dot dot in _model.DotGrid)
            {
                if (dot is T)
                {
                    return true;
                }
            }
        }

        else if (typeof(T) == typeof(Tile) || typeof(T).IsSubclassOf(typeof(Tile)))
        {
            foreach (Tile tile in _model.TileGrid)
            {
                if (tile is T)
                {
                    return true;
                }
            }
        }
        return false;
    }

     public bool IsOnEdgeOfBoard(int col, int row)
    {
        return IsAtBottomOfBoard(col, row) || IsAtLeftOfBoard(col, row) || IsAtRightOfBoard(col, row) || IsAtTopOfBoard(col, row);
    }
     public bool FillBoard(DotsObject[] dotsToSpawn = null, Action onComplete = null)
    {
        var drops = CollectRefillDrops(dotsToSpawn);
        foreach (var drop in drops)
        {
            if (drop.Presenter == null) continue;
            drop.Presenter.Drop(drop.TargetRow);
        }
        onComplete?.Invoke();
        return drops.Count > 0;
    }

    public bool CanHostDot(int col, int row)
    {
        ITilePresenter tile = GetTileAt(col, row);
        
        if (tile != null && !tile.Tile.TileType.IsOpenTile())
        {
            return false;
        }
        
        return true;
    }
   
    public bool CollapseColumn(out List<DotDrop> drops)
    {
        drops = new List<DotDrop>();

        bool dotsDropped = false;
        for (int x = 0; x < Width; x++)
        {
            int writeRow = GetBottomMostRow(x); // next row in this column where a dot is allowed to land

            for (int y = 0; y < Height; y++)
            {
                ITilePresenter tile = GetTileAt(x, y);
                IDotPresenter dot = GetDotAt(x, y);

                if (tile != null && tile.Tile.TileType.IsBlockingTile())
                {
                    writeRow = y + 1;
                    continue;

                }
                if (dot == null)
                {
                    continue;
                }
                // Find the next row ≥ writeRow that can actually host a dot
                int landingRow = writeRow;
                while (landingRow < y && !CanHostDot(x, landingRow))
                {
                    landingRow++;

                }
                
                    // If we found a different valid landing cell, move the dot there
                    if (landingRow != y)
                    {
                        if(!IsValidPosition(new Vector2Int(x, landingRow)))
                        {
                            Debug.LogError($"[BoardPresenter] CollapseColumn: Invalid position {new Vector2Int(x, landingRow)}");
                            continue;
                        }
                        MoveDot(dot.Dot.ID, new Vector2Int(x, landingRow));
                        drops.Add(new DotDrop(dot, landingRow));
                        dotsDropped = true;
                    }
                    // Next dot in this segment must land strictly above the last landing cell
                    writeRow = landingRow + 1;

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
    public List<T> CollectPresenters<T>(List<string> ids) where T : class, IPresenter
    {
        var presenters = new List<T>();
        foreach (var id in ids)
        {
            var presenter = GetEntity(id);
            if (presenter != null && presenter.TryGetPresenter(out T tPresenter))
            {
                presenters.Add(tPresenter);
            }
        }
        return presenters;
    }
    public List<DotDrop> CollectRefillDrops(DotsObject[] dotsToSpawn = null)
    {
        var drops = new List<DotDrop>();
        for (int col = 0; col < Width; col++)
        {
            for (int row = Height - 1; row >= 0; row--)
            {
                ITilePresenter tile = GetTileAt(col, row);
                if (tile != null && tile.Tile.TileType.IsBlockingTile())
                {
                    break;
                }
                if (!CanHostDot(col, row))
                {
                    continue;
                }

                if (GetDotAt(col, row) == null)
                {

                    var position = new Vector2Int(col, row);
                    IDotPresenter dot = SpawnRandomDotAt(position, dotsToSpawn ?? _dotsToSpawn);
                    drops.Add(new DotDrop(dot, row));

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
    /// <summary>
    /// Gets the bottom most row. This is the first row from the bottom of the board that has a dot or a tile that contains a dot.
    /// </summary>
    /// <param name="col">The column to check</param>
    /// <returns>The bottommost row</returns>
    public int GetBottomMostRow(int col)
    {
        for (int row = 0; row < Height; row++)
        {
           
            if (CanHostDot(col, row))
            {
                return row;
            }   
        }
        return 0;
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

 public override string ToString()
    {
        return _model.ToString();
    }

    
}

